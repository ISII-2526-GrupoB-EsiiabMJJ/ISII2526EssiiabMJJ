using AppForSEII2526.API.DTOs.RentalDTOSs;
using AppForSEII2526.API.Models;

namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentalsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<RentalsController> _logger;

    public RentalsController(ApplicationDbContext context, ILogger<RentalsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    [Route("[action]")]
    [ProducesResponseType(typeof(RentalDetailDTO), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult> GetRental(int id)
    {
        if (_context.Rental == null)
        {
            _logger.LogError("Error: Rentals table does not exist");
            return NotFound();
        }

        var rental = await _context.Rental
         .Where(r => r.Id == id)
             .Include(r => r.RentalItems) //join table RentalItems
                .ThenInclude(ri => ri.Device) //then join table Device
                    .ThenInclude(device => device.Model) //then join table Model
         .Select(r => new RentalDetailDTO(r.Id, r.RentalDate, r.Name,
                r.Surname, r.DeliveryAddress,
                (PaymentMethodTypes)r.PaymentMethod,
                r.RentalDateFrom, r.RentalDateTo,
                r.RentalItems
                    .Select(ri => new RentalItemDTO(ri.Device.Id,
                            ri.Device.Model.NameModel,
                            ri.Device.priceForRent)).ToList<RentalItemDTO>()))
         .FirstOrDefaultAsync();


        if (rental == null)
        {
            _logger.LogError($"Error: Rental with id {id} does not exist");
            return NotFound();
        }


        return Ok(rental);
    }

    [HttpPost]
    [Route("[action]")]
    [ProducesResponseType(typeof(RentalDetailDTO), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
    public async Task<ActionResult> CreateRental(RentalForCreateDTO rentalForCreate)
    {
        //any validation defined in PurchaseForCreate is checked before running the method so they don't have to be checked again
        if (rentalForCreate.RentalDateFrom <= DateTime.Today)
            ModelState.AddModelError("RentalDateFrom", "Error! Your rental date must start later than today");

        if (rentalForCreate.RentalDateFrom >= rentalForCreate.RentalDateTo)
            ModelState.AddModelError("RentalDateFrom&RentalDateTo", "Error! Your rental must end later than it starts");

        if (rentalForCreate.RentalItems.Count == 0)
            ModelState.AddModelError("RentalItems", "Error! You must include at least one movie to be rented");

            var user = _context.ApplicationUser.FirstOrDefault(au => au.UserName.Equals(rentalForCreate.CustomerUserName));

            if (user == null)
              ModelState.AddModelError("RentalApplicationUser", "Error! The actual user doesn't exist");

            if (ModelState.ErrorCount > 0)
            return BadRequest(new ValidationProblemDetails(ModelState));


        var deviceModels = rentalForCreate.RentalItems.Select(ri => ri.Model).ToList<string>();

        var devices = _context.Device.Include(m => m.RentedDevices)
            .ThenInclude(ri => ri.Rent)
            .Where(m => deviceModels.Contains(m.Model.NameModel))

            //we use an anonymous type https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/types/anonymous-types
            .Select(m => new {
                m.Id,
                m.Model.NameModel,
                m.quantityForRent,
                m.priceForRent,
                //we count the number of rentalItems that are within the rental period
                NumberOfRentedItems = m.RentedDevices.Count(ri => ri.Rent.RentalDateFrom <= rentalForCreate.RentalDateTo
                        && ri.Rent.RentalDateTo >= rentalForCreate.RentalDateFrom)
            })
            .ToList();

            
              Rental rental = new Rental(rentalForCreate.CustomerUserName, rentalForCreate.CustomerNameSurname,
                user, rentalForCreate.DeliveryAddress, DateTime.Now,
                (AppForSEII2526.API.Models.PaymentMethodTypes)rentalForCreate.PaymentMethod,
                rentalForCreate.RentalDateFrom, rentalForCreate.RentalDateTo, new List<RentDevice>());

            rental.ApplicationUserId = user.Id;
             


        rental.TotalPrice = 0;
        var numDays = (rental.RentalDateTo - rental.RentalDateFrom).TotalDays;


        foreach (var item in rentalForCreate.RentalItems)
        {
            var device = devices.FirstOrDefault(m => m.NameModel.Equals(item.Model));
                //we must check that there is enough quantity to be rented in the database
            if ((device == null) || (device.NumberOfRentedItems >= device.quantityForRent))
            {
                ModelState.AddModelError("RentalItems", $"Error! Movie titled '{item.Model}' is not available for being rented from {rentalForCreate.RentalDateFrom.ToShortDateString()} to {rentalForCreate.RentalDateTo.ToShortDateString()}");
            }
            else
            {
                // rental does not exist in the database yet and does not have a valid Id, so we must relate rentalitem to the object rental
                rental.RentalItems.Add(new RentDevice(device.Id, rental, device.priceForRent, item.DeviceID));
                item.PriceForRent = device.priceForRent;
            }
        }
        rental.TotalPrice = rental.RentalItems.Sum(ri => ri.Price * numDays);


        //if there is any problem because of the available quantity of movies or because the movie does not exist
        if (ModelState.ErrorCount > 0)
        {
            return BadRequest(new ValidationProblemDetails(ModelState));
        }

        _context.Add(rental);

        try
        {
            //we store in the database both rental and its rentalitems
            await _context.SaveChangesAsync();
        }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString()); // Muestra todo, incluyendo InnerException
                var inner = ex.InnerException != null ? ex.InnerException.ToString() : "No inner exception";
                return Conflict("Error: " + inner);
            }

            //it returns rentalDetail
            var rentalDetail = new RentalDetailDTO(rental.Id, rental.RentalDate,
            rental.Name, rental.Surname,
            rental.DeliveryAddress, rentalForCreate.PaymentMethod,
            rental.RentalDateFrom, rental.RentalDateTo,
            rentalForCreate.RentalItems);

        return CreatedAtAction("GetRental", new { id = rental.Id }, rentalDetail);
    }
}

}