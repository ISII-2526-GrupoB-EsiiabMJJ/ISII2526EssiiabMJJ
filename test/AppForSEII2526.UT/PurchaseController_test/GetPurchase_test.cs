using AppForMovies.UT;
using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.PurchaseDTOs;
using AppForSEII2526.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AppForSEII2526.UT.PurchasesController_test
{
    public class GetPurchase_test : AppForSEII25264SqliteUT
    {
        private readonly Purchase _purchase;

        public GetPurchase_test()
        {
            // Modelo asociado al dispositivo que aparecerá en el detalle de la compra.
            var model = new Model(1, "NVIDIA GeForce RTX 5090");

            // Dispositivo comprado. Se inicializa con todos los datos que después deben aparecer
            // en el DTO de detalle: marca, modelo, color, precio, cantidad y descripción.
            var device = new Device(
                1,
                2025,
                QualityType.Hight,
                3,
                5,
                59.99,
                999.99,
                "RTX 5090 Founders Edition",
                "Negro",
                "NVIDIA",
                "Máximo rendimiento en gaming 8K y tareas de IA",
                model,
                new List<RentDevice>(),
                new List<ReviewItem>(),
                new List<PurchaseItem>()
            );

            // Usuario asociado a la compra. El detalle debe mostrar sus datos personales.
            var user = new ApplicationUser("1", "Maria", "Torres", "maria@uclm.es")
            {
                UserName = "maria@uclm.es",
                Email = "maria@uclm.es"
            };

            // Compra almacenada previamente en la base de datos.
            // Incluye usuario, dirección, fecha, método de pago y la lista de líneas de compra.
            _purchase = new Purchase(
                1,
                "maria@uclm.es",
                "Maria",
                "Torres",
                user,
                "Albacete",
                new DateTime(2026, 5, 5, 10, 0, 0, DateTimeKind.Utc),
                new List<PurchaseItem>(),
                PaymentMethod.CreditCard
            );

            // Línea de compra que relaciona el dispositivo con la compra.
            // La cantidad y el precio se usan para validar posteriormente los totales.
            var purchaseItem = new PurchaseItem(
                device,
                999.99m,
                2,
                _purchase)
            {
                Description = device.Description
            };

            // Se añade la línea a la compra y se calculan los totales esperados.
            _purchase.Items.Add(purchaseItem);
            _purchase.TotalPrice = 1999.98m;
            _purchase.TotalQuantity = 2;

            // Se guardan todos los datos necesarios para que GetPurchase pueda recuperar
            // la compra completa con usuario, dispositivo y modelo.
            _context.Add(user);
            _context.Add(model);
            _context.Add(device);
            _context.Add(_purchase);
            _context.SaveChanges();
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetPurchase_OK_test()
        {
            // Arrange
            // Se inicializa el controlador con el contexto que ya contiene una compra guardada.
            var controller = new PurchasesController(
                _context,
                new Mock<ILogger<PurchasesController>>().Object);

            // Act
            // Se solicita el detalle de la compra existente.
            var result = await controller.GetPurchase(_purchase.Id);

            // Assert
            // Al existir la compra, el controlador debe devolver una respuesta OK.
            var okResult = Assert.IsType<OkObjectResult>(result.Result);

            // El cuerpo de la respuesta debe ser un DTO de detalle de compra.
            var purchaseDetail = Assert.IsType<PurchaseDetailDTO>(okResult.Value);

            // Se comprueba que el identificador devuelto corresponde a la compra solicitada.
            Assert.Equal(_purchase.Id, purchaseDetail.Id);

            // Se comprueban los datos personales asociados a la compra.
            Assert.Equal("Maria", purchaseDetail.Name);
            Assert.Equal("Torres", purchaseDetail.Surname);

            // Se comprueba la dirección de entrega almacenada.
            Assert.Equal("Albacete", purchaseDetail.DeliveryAddress);

            // La fecha devuelta debe coincidir con la fecha guardada en la compra.
            Assert.Equal(_purchase.PurchaseDateUtc, purchaseDetail.PurchaseDateUtc);

            // El total debe corresponder a precio * cantidad: 999.99 * 2 = 1999.98.
            Assert.Equal(1999.98, purchaseDetail.TotalPrice, 2);

            // La cantidad total debe coincidir con las unidades compradas.
            Assert.Equal(2, purchaseDetail.TotalQuantity);

            // Solo se espera una línea de compra en el detalle.
            Assert.Single(purchaseDetail.PurchaseItems);

            // Se extrae la línea de compra para validar los datos del dispositivo.
            var item = purchaseDetail.PurchaseItems.First();

            // El dispositivo del detalle debe coincidir con el que se guardó en la compra.
            Assert.Equal(1, item.DeviceId);
            Assert.Equal("NVIDIA", item.Brand);
            Assert.Equal("NVIDIA GeForce RTX 5090", item.Model);
            Assert.Equal("Negro", item.Color);
            Assert.Equal(999.99m, item.Price);
            Assert.Equal(2, item.Quantity);

            // La descripción también debe viajar en el DTO porque forma parte del detalle mostrado al usuario.
            Assert.Equal("Máximo rendimiento en gaming 8K y tareas de IA", item.Description);
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetPurchase_NotFound_test()
        {
            // Arrange
            var controller = new PurchasesController(
                _context,
                new Mock<ILogger<PurchasesController>>().Object);

            // Act
            // Se solicita un identificador que no corresponde a ninguna compra guardada.
            // Este caso comprueba que no se devuelve un detalle cuando el recurso no existe.
            var result = await controller.GetPurchase(999);

            // Assert
            // Si no existe la compra, el controlador debe devolver NotFound.
            Assert.IsType<NotFoundResult>(result.Result);
        }
    }
}