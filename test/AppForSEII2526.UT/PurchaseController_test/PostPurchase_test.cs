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
    public class PostPurchase_test : AppForSEII25264SqliteUT
    {
        private readonly ApplicationUser _user;
        private readonly Device _deviceWithStock;
        private readonly Device _deviceWithoutEnoughStock;

        public PostPurchase_test()
        {
            // Datos base compartidos por los tests. Se crean modelos, dispositivos y usuario
            // para probar el controlador con información real almacenada en la base de datos de pruebas.
            var models = new List<Model>()
            {
                new Model(1, "NVIDIA GeForce RTX 5090"),
                new Model(2, "NVIDIA GeForce RTX 5080")
            };

            // Dispositivo con stock suficiente. Se utiliza en los casos donde la compra debe completarse.
            _deviceWithStock = new Device(
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
                models[0],
                new List<RentDevice>(),
                new List<ReviewItem>(),
                new List<PurchaseItem>()
            );

            // Dispositivo con stock limitado. Permite comprobar que no se acepta una cantidad superior a la disponible.
            _deviceWithoutEnoughStock = new Device(
                2,
                2025,
                QualityType.Medium,
                8,
                1,
                49.99,
                799.99,
                "RTX 5080 Gaming Pro",
                "Plata",
                "MSI",
                "Ideal para gaming 4K con DLSS 4.0",
                models[1],
                new List<RentDevice>(),
                new List<ReviewItem>(),
                new List<PurchaseItem>()
            );

            // Usuario válido para las compras correctas. El UserName debe coincidir con el CustomerUserName del DTO.
            _user = new ApplicationUser("1", "Maria", "Torres", "maria@uclm.es")
            {
                UserName = "maria@uclm.es",
                Email = "maria@uclm.es"
            };

            // Se insertan los datos iniciales en la base de datos en memoria antes de ejecutar cada test.
            _context.Add(_user);
            _context.AddRange(models);
            _context.Add(_deviceWithStock);
            _context.Add(_deviceWithoutEnoughStock);
            _context.SaveChanges();
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task CreatePurchase_OK_test()
        {
            // Arrange
            // Se inicializa el controlador con el contexto de pruebas y un logger simulado.
            var controller = new PurchasesController(
                _context,
                new Mock<ILogger<PurchasesController>>().Object);

            // Compra válida: usuario registrado, dirección informada, método permitido,
            // y un dispositivo existente con stock suficiente.
            var purchaseForCreate = new PurchaseForCreateDTO(
                "maria@uclm.es",
                "Maria",
                "Torres",
                "Albacete",
                PaymentMethod.CreditCard,
                new List<PurchaseItemDTO>
                {
                    new PurchaseItemDTO(
                        _deviceWithStock.Id,
                        _deviceWithStock.Brand,
                        _deviceWithStock.Model.NameModel,
                        _deviceWithStock.Color,
                        999.99m,
                        2,
                        _deviceWithStock.Description)
                });

            // Act
            // Se llama al método del controlador encargado de crear la compra.
            var result = await controller.CreatePurchase(purchaseForCreate);

            // Assert
            // Si los datos son válidos, la respuesta debe ser 201 Created.
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);

            // El recurso creado debe quedar asociado a la acción que permite consultar el detalle de la compra.
            Assert.Equal(nameof(PurchasesController.GetPurchase), createdResult.ActionName);

            // Se obtiene el DTO devuelto para comprobar que contiene el detalle de la compra creada.
            var purchaseDetail = Assert.IsType<PurchaseDetailDTO>(createdResult.Value);

            // Se comprueba que los datos personales devueltos coinciden con los enviados en la petición.
            Assert.Equal("Maria", purchaseDetail.Name);
            Assert.Equal("Torres", purchaseDetail.Surname);
            Assert.Equal("Albacete", purchaseDetail.DeliveryAddress);

            // El precio total debe ser precio unitario * cantidad: 999.99 * 2 = 1999.98.
            Assert.Equal(1999.98, purchaseDetail.TotalPrice, 2);

            // La cantidad total debe coincidir con la suma de unidades compradas.
            Assert.Equal(2, purchaseDetail.TotalQuantity);

            // Solo se ha añadido una línea de compra, correspondiente al único dispositivo solicitado.
            Assert.Single(purchaseDetail.PurchaseItems);

            // Se extrae la línea de compra para comprobar los datos concretos del dispositivo.
            var item = purchaseDetail.PurchaseItems.First();

            // El dispositivo del detalle debe ser el mismo que se incluyó en la compra.
            Assert.Equal(_deviceWithStock.Id, item.DeviceId);
            Assert.Equal("NVIDIA", item.Brand);
            Assert.Equal("NVIDIA GeForce RTX 5090", item.Model);
            Assert.Equal("Negro", item.Color);
            Assert.Equal(999.99m, item.Price);
            Assert.Equal(2, item.Quantity);

            // Se comprueba que la compra se ha guardado realmente en la base de datos.
            Assert.Single(_context.Purchases);

            // También debe haberse guardado una única línea asociada a la compra.
            Assert.Single(_context.PurchaseItems);
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task CreatePurchase_NullPurchase_ReturnsBadRequest()
        {
            // Arrange
            var controller = new PurchasesController(
                _context,
                new Mock<ILogger<PurchasesController>>().Object);

            // Act
            // Se envía una compra nula para comprobar que el controlador corta la ejecución al inicio.
            var result = await controller.CreatePurchase(null!);

            // Assert
            // Una petición sin objeto de compra debe devolver BadRequest.
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);

            // El cuerpo de la respuesta debe contener los errores de validación.
            var validationProblem = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);

            // La clave del error identifica que el problema está en el objeto de compra completo.
            Assert.Contains("Purchase", validationProblem.Errors.Keys);

            // El mensaje debe explicar que no se puede crear una compra a partir de un objeto nulo.
            Assert.Contains(
                "La compra no puede ser nula",
                validationProblem.Errors["Purchase"]);
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task CreatePurchase_DeviceNotFound_ReturnsBadRequest()
        {
            // Arrange
            var controller = new PurchasesController(
                _context,
                new Mock<ILogger<PurchasesController>>().Object);

            // El DTO contiene un DeviceId inexistente. El resto de datos son válidos
            // para aislar el error y comprobar únicamente la validación del dispositivo.
            var purchaseForCreate = new PurchaseForCreateDTO(
                "maria@uclm.es",
                "Maria",
                "Torres",
                "Albacete",
                PaymentMethod.CreditCard,
                new List<PurchaseItemDTO>
                {
                    new PurchaseItemDTO(
                        999,
                        "Marca inexistente",
                        "Modelo inexistente",
                        "Negro",
                        999.99m,
                        1,
                        "Descripción")
                });

            // Act
            var result = await controller.CreatePurchase(purchaseForCreate);

            // Assert
            // La compra debe rechazarse porque no se encuentra el dispositivo en la base de datos.
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);

            // Se obtiene el detalle de validación para comprobar la clave y el mensaje.
            var validationProblem = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);

            // La clave permite saber que el error está asociado a la búsqueda del dispositivo.
            Assert.Contains("Device", validationProblem.Errors.Keys);

            // El mensaje debe indicar el identificador concreto que no se ha encontrado.
            Assert.Contains(
                "El dispositivo con id 999 no existe",
                validationProblem.Errors["Device"]);

            // No se debe persistir nada si la compra contiene dispositivos inválidos.
            Assert.Empty(_context.Purchases);
            Assert.Empty(_context.PurchaseItems);
        }

        public static IEnumerable<object[]> TestCasesFor_CreatePurchase_BadRequest()
        {
            // Caso 1: compra sin dispositivos.
            // Aunque el usuario y la dirección sean válidos, la compra no puede estar vacía.
            yield return new object[]
            {
                new PurchaseForCreateDTO(
                    "maria@uclm.es",
                    "Maria",
                    "Torres",
                    "Albacete",
                    PaymentMethod.CreditCard,
                    new List<PurchaseItemDTO>()),
                "PurchaseItems"
            };

            // Caso 2: usuario inexistente.
            // El resto de datos son válidos para comprobar únicamente la validación del usuario.
            yield return new object[]
            {
                new PurchaseForCreateDTO(
                    "noexiste@uclm.es",
                    "Maria",
                    "Torres",
                    "Albacete",
                    PaymentMethod.CreditCard,
                    new List<PurchaseItemDTO>
                    {
                        new PurchaseItemDTO(
                            1,
                            "NVIDIA",
                            "NVIDIA GeForce RTX 5090",
                            "Negro",
                            999.99m,
                            1,
                            "Descripción")
                    }),
                "PurchaseApplicationUser"
            };

            // Caso 3: dirección no informada.
            // La compra necesita una dirección de entrega para poder completarse.
            yield return new object[]
            {
                new PurchaseForCreateDTO(
                    "maria@uclm.es",
                    "Maria",
                    "Torres",
                    "",
                    PaymentMethod.CreditCard,
                    new List<PurchaseItemDTO>
                    {
                        new PurchaseItemDTO(
                            1,
                            "NVIDIA",
                            "NVIDIA GeForce RTX 5090",
                            "Negro",
                            999.99m,
                            1,
                            "Descripción")
                    }),
                "DeliveryAddress"
            };

            // Caso 4: método de pago no permitido.
            // Se comprueba que Cash no sea aceptado para este caso de uso.
            yield return new object[]
            {
                new PurchaseForCreateDTO(
                    "maria@uclm.es",
                    "Maria",
                    "Torres",
                    "Albacete",
                    PaymentMethod.Cash,
                    new List<PurchaseItemDTO>
                    {
                        new PurchaseItemDTO(
                            1,
                            "NVIDIA",
                            "NVIDIA GeForce RTX 5090",
                            "Negro",
                            999.99m,
                            1,
                            "Descripción")
                    }),
                "PaymentMethod"
            };

            // Caso 5: cantidad inválida.
            // La cantidad de cada dispositivo debe ser al menos 1.
            yield return new object[]
            {
                new PurchaseForCreateDTO(
                    "maria@uclm.es",
                    "Maria",
                    "Torres",
                    "Albacete",
                    PaymentMethod.CreditCard,
                    new List<PurchaseItemDTO>
                    {
                        new PurchaseItemDTO(
                            1,
                            "NVIDIA",
                            "NVIDIA GeForce RTX 5090",
                            "Negro",
                            999.99m,
                            0,
                            "Descripción")
                    }),
                "Quantity"
            };

            // Caso 6: stock insuficiente.
            // El dispositivo existe, pero se solicita una cantidad superior a la disponible.
            yield return new object[]
            {
                new PurchaseForCreateDTO(
                    "maria@uclm.es",
                    "Maria",
                    "Torres",
                    "Albacete",
                    PaymentMethod.CreditCard,
                    new List<PurchaseItemDTO>
                    {
                        new PurchaseItemDTO(
                            2,
                            "MSI",
                            "NVIDIA GeForce RTX 5080",
                            "Plata",
                            799.99m,
                            5,
                            "Descripción")
                    }),
                "Stock"
            };
        }

        [Theory]
        [MemberData(nameof(TestCasesFor_CreatePurchase_BadRequest))]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task CreatePurchase_BadRequest_test(
            PurchaseForCreateDTO purchaseForCreate,
            string expectedErrorKey)
        {
            // Arrange
            var controller = new PurchasesController(
                _context,
                new Mock<ILogger<PurchasesController>>().Object);

            // Act
            // Se ejecuta la creación con un DTO inválido recibido desde los casos de prueba.
            var result = await controller.CreatePurchase(purchaseForCreate);

            // Assert
            // Todos los casos de este Theory deben devolver BadRequest.
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);

            // El cuerpo de la respuesta debe contener los errores de validación generados por el controlador.
            var validationProblem = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);

            // Cada caso indica la clave concreta que debe aparecer en el ModelState.
            Assert.Contains(expectedErrorKey, validationProblem.Errors.Keys);
        }
    }
}
