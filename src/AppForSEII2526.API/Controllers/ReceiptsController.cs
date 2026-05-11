using AppForSEII2526.API.Data;
using AppForSEII2526.API.DTOs.RepairDTOs;
using AppForSEII2526.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace AppForSEII2526.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceiptsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ReceiptsController> _logger;

        public ReceiptsController(ApplicationDbContext context, ILogger<ReceiptsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Receipts/GetRepairsForReceipt
        // Devuelve las reparaciones disponibles para crear un recibo.
        // Permite filtrar por nombre de reparación y por escala.
        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(IList<RepairForReceiptDTO>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IList<RepairForReceiptDTO>>> GetRepairsForReceipt(
            string? name,
            string? scale)
        {
            if (!string.IsNullOrWhiteSpace(name))
                name = name.Trim().ToLower();

            if (!string.IsNullOrWhiteSpace(scale))
                scale = scale.Trim().ToLower();

            var query = _context.Repairs
                .Include(r => r.Scale)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(r => r.Name.ToLower().Contains(name));
            }

            if (!string.IsNullOrWhiteSpace(scale))
            {
                query = query.Where(r => r.Scale.Name.ToLower().Contains(scale));
            }

            var repairs = await query
                .OrderBy(r => r.Name)
                .Select(r => new RepairForReceiptDTO(
                    r.Id,
                    r.Name,
                    r.Description,
                    r.Cost,
                    r.Scale.Name))
                .ToListAsync();

            return Ok(repairs);
        }

        // GET: api/Receipts/GetReceipt/5
        // Devuelve el detalle completo de un recibo de reparación.
        [HttpGet]
        [Route("[action]/{id:int}")]
        [ProducesResponseType(typeof(ReceiptDetailDTO), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<ReceiptDetailDTO>> GetReceipt(int id)
        {
            var receipt = await _context.Receipts
                .Include(r => r.ReceiptItems)
                    .ThenInclude(ri => ri.Repair)
                        .ThenInclude(repair => repair.Scale)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (receipt == null)
            {
                _logger.LogWarning("Receipt with id {ReceiptId} does not exist", id);
                return NotFound();
            }

            var receiptDetail = MapReceiptToReceiptDetailDTO(receipt);

            return Ok(receiptDetail);
        }

        // POST: api/Receipts/CreateReceipt
        // Crea un recibo de reparación a partir de las reparaciones seleccionadas.
        [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(ReceiptDetailDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult<ReceiptDetailDTO>> CreateReceipt(
            [FromBody] ReceiptForCreateDTO receiptForCreate)
        {
            if (receiptForCreate == null)
            {
                ModelState.AddModelError("Receipt", "The receipt cannot be null");
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            if (string.IsNullOrWhiteSpace(receiptForCreate.CustomerUserName))
            {
                ModelState.AddModelError("CustomerUserName", "The customer username is required");
            }

            if (string.IsNullOrWhiteSpace(receiptForCreate.CustomerNameSurname))
            {
                ModelState.AddModelError("CustomerNameSurname", "The customer name and surname are required");
            }

            if (string.IsNullOrWhiteSpace(receiptForCreate.DeliveryAddress))
            {
                ModelState.AddModelError("DeliveryAddress", "The delivery address is required");
            }

            if (receiptForCreate.ReceiptItems == null || receiptForCreate.ReceiptItems.Count == 0)
            {
                ModelState.AddModelError("ReceiptItems", "You must include at least one repair");
            }
            else
            {
                foreach (var item in receiptForCreate.ReceiptItems)
                {
                    if (string.IsNullOrWhiteSpace(item.Model))
                    {
                        ModelState.AddModelError("Model", "The device model is required");
                    }

                    if (item.RepairId <= 0)
                    {
                        ModelState.AddModelError("RepairId", "The repair id must be greater than 0");
                    }
                }
            }

            if (ModelState.ErrorCount > 0)
            {
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(au => au.UserName == receiptForCreate.CustomerUserName);

            if (user == null)
            {
                ModelState.AddModelError("ReceiptApplicationUser", "Error! User is not registered");
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            var repairIds = receiptForCreate.ReceiptItems
                .Select(ri => ri.RepairId)
                .Distinct()
                .ToList();

            var repairs = await _context.Repairs
                .Include(r => r.Scale)
                .Where(r => repairIds.Contains(r.Id))
                .ToListAsync();

            foreach (var item in receiptForCreate.ReceiptItems)
            {
                if (!repairs.Any(r => r.Id == item.RepairId))
                {
                    ModelState.AddModelError(
                        "Repair",
                        $"The repair with id {item.RepairId} does not exist");
                }
            }

            if (ModelState.ErrorCount > 0)
            {
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            decimal totalPrice = receiptForCreate.ReceiptItems
                .Sum(item => repairs.First(r => r.Id == item.RepairId).Cost);

            var receipt = new Receipt(
                receiptForCreate.CustomerNameSurname,
                receiptForCreate.DeliveryAddress,
                receiptForCreate.PaymentMethod,
                DateTime.UtcNow,
                totalPrice,
                user);

            foreach (var item in receiptForCreate.ReceiptItems)
            {
                var repair = repairs.First(r => r.Id == item.RepairId);

                var receiptItem = new ReceiptItem
                {
                    Model = item.Model,
                    RepairId = repair.Id,
                    Repair = repair,
                    Receipt = receipt
                };

                receipt.ReceiptItems.Add(receiptItem);
            }

            _context.Receipts.Add(receipt);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving the repair receipt");
                return Conflict("Error saving the repair receipt: " + ex.Message);
            }

            var receiptDetail = MapReceiptToReceiptDetailDTO(receipt);

            return CreatedAtAction(
                nameof(GetReceipt),
                new { id = receipt.Id },
                receiptDetail);
        }

        private static ReceiptDetailDTO MapReceiptToReceiptDetailDTO(Receipt receipt)
        {
            return new ReceiptDetailDTO(
                receipt.Id,
                receipt.CustomerNameSurname,
                receipt.DeliveryAddress,
                receipt.PaymentMethod,
                receipt.ReceiptDate,
                receipt.TotalPrice,
                receipt.ReceiptItems.Select(ri => new ReceiptItemDTO(
                    ri.Model,
                    ri.RepairId,
                    ri.Repair.Name,
                    ri.Repair.Description,
                    ri.Repair.Cost,
                    ri.Repair.Scale.Name)).ToList());
        }
    }
}