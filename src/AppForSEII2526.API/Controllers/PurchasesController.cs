using AppForSEII2526.API.DTOs.PurchaseDTOs;
using AppForSEII2526.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchasesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PurchasesController> _logger;

        public PurchasesController(ApplicationDbContext context, ILogger<PurchasesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(PurchaseDetailDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<PurchaseDetailDTO>> GetPurchase(int id)
        {
            var purchase = await _context.Purchases
                .Include(p => p.ApplicationUser)
                .Include(p => p.Items)
                    .ThenInclude(pi => pi.Device)
                        .ThenInclude(d => d.Model)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (purchase == null)
            {
                _logger.LogWarning("Compra con id {PurchaseId} no existe", id);
                return NotFound();
            }

            var purchaseDetail = new PurchaseDetailDTO(
                purchase.Id,
                purchase.Name,
                purchase.Surname,
                purchase.DeliveryAddress,
                purchase.PurchaseDateUtc,
                Convert.ToDouble(purchase.TotalPrice),
                purchase.TotalQuantity,
                purchase.Items.Select(pi => new PurchaseItemDTO(
                    pi.DeviceId,
                    pi.Device.Brand,
                    pi.Device.Model.NameModel,
                    pi.Device.Color,
                    pi.Price,
                    pi.Quantity,
                    pi.Description
                )).ToList()
            );

            return Ok(purchaseDetail);
        }
    }
}