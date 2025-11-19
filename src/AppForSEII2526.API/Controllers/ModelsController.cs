using AppForSEII2526.API.DTOs;
using AppForSEII2526.API.DTOs.DeviceDTOs.DevideRentalDTOs;

namespace AppForSEII2526.API.Controllers
{
    [Route("api/Rentals")]
    [ApiController]
    public class ModelsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private ILogger _logger;

        public ModelsController(ApplicationDbContext context, ILogger<ModelsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<DeviceRentalDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IList<DeviceRentalDTO>>> GetDevices(string? model, double? precio)
        {
            IList<DeviceRentalDTO> models = (IList<DeviceRentalDTO>)await _context.Device
                .Include(d => d.Model)
                .Include(d => d.RentedDevices).ThenInclude(re => re.Rent)

                .Where(dispositivo => (precio == null || dispositivo.priceForRent == precio)
                                      && (model == null || dispositivo.Model.NameModel.ToLower().Contains(model.ToLower())))

                .OrderBy(d => d.Name)
                .Select(d => new DeviceRentalDTO(
                        d.Id,
                        d.Year,
                        d.priceForRent,
                        d.Name,
                        d.Model.NameModel,
                        d.Color,
                        d.Brand

                        )
                )
                .ToListAsync();

            return Ok(models);
        }
    }

}