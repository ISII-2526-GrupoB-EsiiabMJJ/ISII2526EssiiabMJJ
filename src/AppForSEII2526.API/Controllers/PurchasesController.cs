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

		[HttpPost]
		[Route("[action]")]
		[ProducesResponseType(typeof(PurchaseDetailDTO), (int)HttpStatusCode.Created)]
		[ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
		[ProducesResponseType((int)HttpStatusCode.Conflict)]
		public async Task<ActionResult<PurchaseDetailDTO>> CreatePurchase([FromBody] PurchaseForCreateDTO purchaseForCreate)
		{
			if (purchaseForCreate == null)
			{
				ModelState.AddModelError("Purchase", "La compra no puede ser nula");
				return BadRequest(new ValidationProblemDetails(ModelState));
			}

			if (purchaseForCreate.PurchaseItems == null || purchaseForCreate.PurchaseItems.Count == 0)
			{
				ModelState.AddModelError(
					"PurchaseItems",
					"Debe incluir al menos un dispositivo en la compra");
			}

			if (string.IsNullOrWhiteSpace(purchaseForCreate.DeliveryAddress))
			{
				ModelState.AddModelError(
					"DeliveryAddress",
					"La dirección de entrega es obligatoria");
			}

			if (purchaseForCreate.PaymentMethod == PaymentMethod.Cash)
			{
				ModelState.AddModelError(
					"PaymentMethod",
					"El pago en efectivo no está permitido para compras");
			}

			if (purchaseForCreate.PurchaseItems != null)
			{
				foreach (var item in purchaseForCreate.PurchaseItems)
				{
					if (item.Quantity < 1)
					{
						ModelState.AddModelError(
							"Quantity",
							$"La cantidad del dispositivo con id {item.DeviceId} debe ser mayor que 0");
					}
				}
			}

			if (ModelState.ErrorCount > 0)
			{
				return BadRequest(new ValidationProblemDetails(ModelState));
			}

            var user = await _context.Users
                .FirstOrDefaultAsync(au => au.UserName == purchaseForCreate.CustomerUserName);

            if (user == null)
            {
                ModelState.AddModelError(
                    "PurchaseApplicationUser",
                    "El usuario no está registrado");

                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            var purchase = new Purchase(
                purchaseForCreate.CustomerUserName,
                            purchaseForCreate.Name,
				purchaseForCreate.Surname,
				user,
				purchaseForCreate.DeliveryAddress,
				DateTime.UtcNow,
				new List<PurchaseItem>(),
				purchaseForCreate.PaymentMethod
			);

			foreach (var item in purchaseForCreate.PurchaseItems)
			{
				var device = await _context.Device
					.Include(d => d.Model)
					.FirstOrDefaultAsync(d => d.Id == item.DeviceId);

				if (device == null)
				{
					ModelState.AddModelError(
						"Device",
						$"El dispositivo con id {item.DeviceId} no existe");

					continue;
				}

				if (device.quantityForPurchase < item.Quantity)
				{
					ModelState.AddModelError(
						"Stock",
						$"No hay stock suficiente para el dispositivo con id {item.DeviceId}");

					continue;
				}

				var purchaseItem = new PurchaseItem(
					device,
					item.Price,
					item.Quantity,
					purchase)
				{
					Description = item.Description
				};

				purchase.Items.Add(purchaseItem);
			}

			if (ModelState.ErrorCount > 0)
			{
				return BadRequest(new ValidationProblemDetails(ModelState));
			}

			purchase.TotalPrice = purchase.Items.Sum(pi => pi.Price * pi.Quantity);
			purchase.TotalQuantity = purchase.Items.Sum(pi => pi.Quantity);

			_context.Purchases.Add(purchase);

			try
			{
				await _context.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error al guardar la compra");
				return Conflict("Error al guardar la compra: " + ex.Message);
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

			return CreatedAtAction(
				nameof(GetPurchase),
				new { id = purchase.Id },
				purchaseDetail);
		}
	}
}