using AppForSEII2526.API.DTOs.PurchaseDTOs;
using AppForSEII2526.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.DependencyResolver;
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
            // Se obtiene la compra junto con el usuario y los dispositivos asociados,
            // ya que el detalle necesita mostrar toda esa información en la respuesta.
            var purchase = await _context.Purchases
                .Include(p => p.ApplicationUser)
                .Include(p => p.Items)
                    .ThenInclude(pi => pi.Device)
                        .ThenInclude(d => d.Model)
                .FirstOrDefaultAsync(p => p.Id == id);

            // Si no existe ninguna compra con ese identificador, se devuelve 404.
            if (purchase == null)
            {
                _logger.LogWarning("Compra con id {PurchaseId} no existe", id);
                return NotFound();
            }

            // Se transforma la entidad Purchase en un DTO de detalle para no devolver
            // directamente las entidades de la base de datos.
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
            // Se comprueba que la petición incluya un objeto de compra válido.
            if (purchaseForCreate == null)
            {
                ModelState.AddModelError("Purchase", "La compra no puede ser nula");
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            // Una compra debe contener al menos un dispositivo seleccionado.
            if (purchaseForCreate.PurchaseItems == null || purchaseForCreate.PurchaseItems.Count == 0)
            {
                ModelState.AddModelError(
                    "PurchaseItems",
                    "Debe incluir al menos un dispositivo en la compra");
            }

            // La dirección es necesaria para poder completar la compra.
            if (string.IsNullOrWhiteSpace(purchaseForCreate.DeliveryAddress))
            {
                ModelState.AddModelError(
                    "DeliveryAddress",
                    "La dirección de entrega es obligatoria");
            }

            // En este caso de uso solo se permiten métodos de pago online.
            if (purchaseForCreate.PaymentMethod == PaymentMethod.Cash)
            {
                ModelState.AddModelError(
                    "PaymentMethod",
                    "El pago en efectivo no está permitido para compras");
            }

            // Se valida que todas las cantidades solicitadas sean positivas.
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

            // Si las validaciones iniciales fallan, no se continúa con la creación.
            if (ModelState.ErrorCount > 0)
            {
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            // Se comprueba que el usuario indicado en la compra exista en la base de datos.
            var user = await _context.Users
                .FirstOrDefaultAsync(au => au.UserName == purchaseForCreate.CustomerUserName);

            if (user == null)
            {
                ModelState.AddModelError(
                    "PurchaseApplicationUser",
                    "El usuario no está registrado");

                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            // Se crea la entidad principal de compra con los datos generales del formulario.
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

            // Se procesa cada dispositivo solicitado para comprobar que existe y que hay stock suficiente.
            foreach (var item in purchaseForCreate.PurchaseItems)
            {
                var device = await _context.Device
                    .Include(d => d.Model)
                    .FirstOrDefaultAsync(d => d.Id == item.DeviceId);

                // Si el dispositivo no existe, se informa del error y se continúa revisando el resto.
                if (device == null)
                {
                    ModelState.AddModelError(
                        "Device",
                        $"El dispositivo con id {item.DeviceId} no existe");

                    continue;
                }

                if (device.Brand.Equals("ASUS", StringComparison.OrdinalIgnoreCase) && purchaseForCreate.PaymentMethod == PaymentMethod.PayPal)
                {
                    ModelState.AddModelError(
                        "PaymentMethod",
                        $"El dispositivo con marca {device.Brand} no se puede pagar con PayPal");

                    continue;
                }

                // No se permite comprar una cantidad superior al stock disponible.
                if (device.quantityForPurchase < item.Quantity)
                {
                    ModelState.AddModelError(
                        "Stock",
                        $"No hay stock suficiente para el dispositivo con id {item.DeviceId}");

                    continue;
                }
                
                if(device.quantityForPurchase< item.Quantity)
                {
                    ModelState.AddModelError(
                        "Stock",
                        $"El numero de dispositivos que solicita no esta disponible");

                    continue;
                }
                
                if(device.quantityForPurchase< item.Quantity)
                {
                    ModelState.AddModelError(
                        "Stock",
                        $"El numero de dispositivos que solicita no esta disponible");

                    continue;
                }

                device.quantityForPurchase -= item.Quantity; //resto al stock la cantidad comprada
                // Se crea la línea de compra asociada al dispositivo encontrado.
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

            // Si durante la revisión de dispositivos aparece algún error,
            // se cancela la creación de la compra.
            if (ModelState.ErrorCount > 0)
            {
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

            // Los totales se calculan a partir de las líneas de compra definitivas.
            purchase.TotalPrice = purchase.Items.Sum(pi => pi.Price * pi.Quantity);
            purchase.TotalQuantity = purchase.Items.Sum(pi => pi.Quantity);

            _context.Purchases.Add(purchase);

            try
            {
                // Se guarda la compra junto con sus líneas asociadas.
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar la compra");
                return Conflict("Error al guardar la compra: " + ex.Message);
            }

            // Se prepara el DTO de respuesta con el detalle de la compra recién creada.
            var purchaseDetail = new PurchaseDetailDTO(
                purchase.Id,
                purchase.Name,
                purchase.Surname,
                purchase.DeliveryAddress,
                purchase.PurchaseDateUtc,
                Convert.ToDouble(purchase.TotalPrice),
                purchase.TotalQuantity,
                //purchase.PaymentMethod,
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

            // Se devuelve 201 Created indicando la acción desde la que puede consultarse el recurso creado.
            return CreatedAtAction(
                nameof(GetPurchase),
                new { id = purchase.Id },
                purchaseDetail);
        }
    }
}
