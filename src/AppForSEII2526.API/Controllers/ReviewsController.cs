using AppForSEII2526.API.DTOs;
using AppForSEII2526.API.DTOs.ReviewDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using System.Linq;
using System.Net;

namespace AppForSEII2526.API.Controllers
{

    [Route("api/Review")]
    [ApiController]
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private ILogger _logger;

        public ReviewsController(ApplicationDbContext context, ILogger<ReviewsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ReviewDetailsDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<ReviewDetailsDTO>> ReviewDetails(int id)
        {
            if (_context.Review == null)
            {
                _logger.LogError("Error: No existe ninguna tabla con datos de reseñas");
                return NotFound();
            }

            var reviewsList = await _context.Review
                .Where(review => review.ReviewId == id)
                .Include(review => review.ReviewItems)
                .ThenInclude(reviewItem => reviewItem.Device)
                .ThenInclude(device => device.Model)
                .Select(review => new ReviewDetailsDTO(
                        review.ReviewId,
                        review.CustomerName,
                        review.CustomerCountry,
                        review.ReviewTitle,
                        review.DateOfReview,
                        review.ReviewItems.Select(revItem => new ReviewDeviceDTO(
                            revItem.DeviceId,
                            revItem.Device.Name,
                            revItem.Device.Model.NameModel,
                            revItem.Device.Year,
                            revItem.Rating,
                            revItem.Comment)
                        ).ToList()
                    )
                ).FirstOrDefaultAsync();
            if (reviewsList == null)
            {
                _logger.LogError($"Error: La reseña con id {id} no existe");
                return NotFound();
            }

            return Ok(reviewsList);

        }

        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(ReviewDetailsDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<ReviewDetailsDTO>> CreateReview([FromBody] CreateReviewDTO reviewCrear)
        {
            if (_context.Review == null || _context.Device == null)
            {
                _logger.LogError("Error: Las tablas necesarias para crear una reseña no existen");
                return Problem("Las entidades 'Review' o 'Device' son null en ApplicationDbContext.");
            }

            if (reviewCrear.ReviewItems == null || reviewCrear.ReviewItems.Count < 1)
            {
                ModelState.AddModelError("ReviewItems", "Debes incluir al menos un dispositivo reseñado.");
                return BadRequest(new ValidationProblemDetails(ModelState));
            }


            // Validar existencia de dispositivos y construir ReviewItems
            var reviewItems = new List<ReviewItem>();
            int reviewDevicesCounter = 0, ratingCounter = 0;
            foreach (var item in reviewCrear.ReviewItems)
            {
                if (item.Rating == -1)
                {
                    ModelState.AddModelError("ReviewItems", "You must rate every chosen device for this review.");
                    return BadRequest(new ValidationProblemDetails(ModelState));
                }
                reviewDevicesCounter++;
                ratingCounter += item.Rating;
                var device = await _context.Device
                    .Include(d => d.Model)
                    .FirstOrDefaultAsync(d => d.Id == item.DeviceId);

                if (device == null)
                {
                    ModelState.AddModelError("Device", $"El dispositivo con ID {item.DeviceId} no existe.");
                }
                else
                {
                    reviewItems.Add(new ReviewItem
                    {
                        Rating = item.Rating,
                        Comment = item.Comment,
                        Device = device
                    });
                }
            }

            int overallRatingCalculated = ratingCounter / reviewDevicesCounter;

            if (ModelState.ErrorCount > 0)
            {
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            var newReview = new Review
            {
                CustomerId = reviewCrear.CustomerId,
                OverallRating = overallRatingCalculated,
                ReviewTitle = reviewCrear.ReviewTitle,
                CustomerCountry = reviewCrear.CustomerCountry,
                CustomerName = reviewCrear.CustomerName,
                DateOfReview = DateTime.Now,
                ReviewItems = reviewItems
            };
            newReview.ApplicationUserId = newReview.CustomerId.ToString();

            _context.Review.Add(newReview);
            await _context.SaveChangesAsync();

            var reviewDTO = new ReviewDetailsDTO(
                newReview.ReviewId,
                newReview.CustomerName,
                newReview.CustomerCountry,
                newReview.ReviewTitle,
                newReview.DateOfReview,
                reviewItems.Select(ri => new ReviewDeviceDTO(
                    ri.DeviceId,
                    ri.Device.Name,
                    ri.Device.Model.NameModel,
                    ri.Device.Year,
                    ri.Rating,
                    ri.Comment
                )).ToList()
            );

            return CreatedAtAction("ReviewDetails", new { id = newReview.ReviewId }, reviewDTO);
        }

    }
}