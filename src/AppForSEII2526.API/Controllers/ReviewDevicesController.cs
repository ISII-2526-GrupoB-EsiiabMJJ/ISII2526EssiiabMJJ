using AppForSEII2526.API.DTOs.ReviewDTOs;

namespace AppForSEII2526.API.Controllers
{
    [Route("api/Reviews/Device")]
    [ApiController]
    public class DevicesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private ILogger _logger;

        public DevicesController(ApplicationDbContext context, ILogger<DevicesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<DeviceDetailsDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IList<DeviceDetailsDTO>>> DeviceInfo
            (string? brand, int? year)
        {
            IList<DeviceDetailsDTO> deviceList = (IList<DeviceDetailsDTO>)await _context.Device
                .Include(d => d.Model)
                .Include(d => d.ReviewItems).ThenInclude(re => re.Review)

                .Where(device => (year == null || device.Year == year)
                                 && (brand == null || device.Brand.ToLower().Contains(brand.ToLower()))
                                 //&& (brand == null || device.Model.NameModel.ToLower().Contains(brand.ToLower()))
                                 )

                .OrderBy(d => d.Name)
                .Select(d => new DeviceDetailsDTO(
                        d.Id,
                        d.Name,
                        d.Brand,
                        d.Color,
                        d.Year,
                        d.Model.NameModel
                    )
                )
                .ToListAsync();

            return Ok(deviceList);
        }
    }

}