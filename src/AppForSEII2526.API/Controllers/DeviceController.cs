using AppForSEII2526.API.DTOs.DeviceDTOs.DevideRentalDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DeviceController> _logger;

        public DeviceController(ApplicationDbContext context, ILogger<DeviceController> logger)
        {
            _context = context;
            _logger = logger;
        }

        //-------------------------------------------------
        // GET: api/Device/rental
        // Lista todos los dispositivos filtrando por marca o modelo
        //-------------------------------------------------
        [HttpGet("rental")]
        [ProducesResponseType(typeof(IList<DeviceRentalDTO>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetDeviceForRental(
            string? deviceBrand,
            string? deviceModel)
        {
            // Validación básica
            if (!string.IsNullOrEmpty(deviceBrand))
                deviceBrand = deviceBrand.Trim().ToLower();

            if (!string.IsNullOrEmpty(deviceModel))
                deviceModel = deviceModel.Trim().ToLower();

            // Consulta
            var devices = await _context.Device
                .Include(d => d.Model)
                .Include(d => d.RentedDevices)
                    .ThenInclude(rd => rd.Rent)
                .Where(d =>
                    (string.IsNullOrEmpty(deviceBrand) || d.Brand.ToLower().Contains(deviceBrand)) &&
                    (string.IsNullOrEmpty(deviceModel) || d.Model.NameModel.ToLower().Contains(deviceModel))
                )
                .OrderBy(d => d.Brand)
                .Select(d => new DeviceRentalDTO(
                    d.Id,
                    d.Year,
                    d.priceForRent,
                    d.Name,
                    d.Model.NameModel,
                    d.Color,
                    d.Brand))
                .ToListAsync();

            return Ok(devices);
        }

        //-------------------------------------------------
        // GET: api/Device/rentalByDate
        // Filtra dispositivos disponibles entre dos fechas
        //-------------------------------------------------
        [HttpGet("rentalByDate")]
        [ProducesResponseType(typeof(IList<DeviceRentalDTO>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetDeviceForRentalByDate(DateTime fromDate, DateTime toDate)
        {
            if (fromDate > toDate)
            {
                return BadRequest(new ValidationProblemDetails
                {
                    Title = "Invalid date range",
                    Status = (int)HttpStatusCode.BadRequest,
                    Detail = "fromDate must be earlier than toDate"
                });
            }

            var devices = await _context.Device
                .Include(d => d.Model)
                .Include(d => d.RentedDevices)
                    .ThenInclude(rd => rd.Rent)
                .Where(d => d.RentedDevices
                        .All(rd => rd.Rent.RentalDateTo < fromDate || rd.Rent.RentalDateFrom > toDate))
                .OrderBy(d => d.Brand)
                .Select(d => new DeviceRentalDTO(
                    d.Id,
                    d.Year,
                    d.priceForRent,
                    d.Name,
                    d.Model.NameModel,
                    d.Color,
                    d.Brand))
                .ToListAsync();

            return Ok(devices);
        }
    }
}
