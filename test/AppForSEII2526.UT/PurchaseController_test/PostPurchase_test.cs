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
        private readonly Device _deviceHuawei;
        private readonly Device _deviceHuawei1;
        private readonly Device _deviceXiaomi;
        private readonly Device _deviceXiaomi1;

        public PostPurchase_test()
        {
            var models = new List<Model>()
            {
                new Model(1, "NVIDIA GeForce RTX 5090"),
                new Model(2, "NVIDIA GeForce RTX 5080"),
                new Model(3, "2026"),
                new Model(4, "Huawei 2026_1"),
                new Model(5, "2026"),
                new Model(6, "Xiaomi 2026_1")
            };

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

            _deviceHuawei = new Device(
                3,
                2026,
                QualityType.Medium,
                5,
                10,
                49.99,
                799.99,
                "2026",
                "Plata",
                "Huawei", 
                "Descripcion",
                models[2],
                new List<RentDevice>(),
                new List<ReviewItem>(),
                new List<PurchaseItem>()
            );

            _deviceHuawei1 = new Device(
                4,
                2026,
                QualityType.Medium,
                5,
                10,
                49.99,
                799.99,
                "Huawei 2026_1",
                "Plata",
                "Huawei_1",
                "Descripcion",
                models[3],
                new List<RentDevice>(),
                new List<ReviewItem>(),
                new List<PurchaseItem>()
            );

            _deviceXiaomi = new Device(
                5,
                2026,
                QualityType.Medium,
                5,
                10,
                49.99,
                799.99,
                "2026",
                "Plata",
                "Xiaomi",
                "Descripcion",
                models[4],
                new List<RentDevice>(),
                new List<ReviewItem>(),
                new List<PurchaseItem>()
            );

            _deviceXiaomi1 = new Device(
                6,
                2026,
                QualityType.Medium,
                5,
                10,
                49.99,
                799.99,
                "Xiaomi 2026_1",
                "Plata",
                "Xiaomi_1",
                "Descripcion",
                models[5],
                new List<RentDevice>(),
                new List<ReviewItem>(),
                new List<PurchaseItem>()
            );
            _user = new ApplicationUser("1", "Maria", "Torres", "maria@uclm.es")
            {
                UserName = "maria@uclm.es",
                Email = "maria@uclm.es"
            };

            _context.Add(_user);
            _context.AddRange(models);
            _context.Add(_deviceWithStock);
            _context.Add(_deviceWithoutEnoughStock);

            _context.Add(_deviceHuawei);
            _context.Add(_deviceHuawei1);
            _context.Add(_deviceXiaomi);
            _context.Add(_deviceXiaomi1);
            _context.SaveChanges();
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task CreatePurchase_OK_test()
        {
            var controller = new PurchasesController(
                _context,
                new Mock<ILogger<PurchasesController>>().Object);

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

            var result = await controller.CreatePurchase(purchaseForCreate);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(PurchasesController.GetPurchase), createdResult.ActionName);

            var purchaseDetail = Assert.IsType<PurchaseDetailDTO>(createdResult.Value);

            Assert.Equal("Maria", purchaseDetail.Name);
            Assert.Equal("Torres", purchaseDetail.Surname);
            Assert.Equal("Albacete", purchaseDetail.DeliveryAddress);
            Assert.Equal(1999.98, purchaseDetail.TotalPrice, 2);
            Assert.Equal(2, purchaseDetail.TotalQuantity);
            Assert.Single(purchaseDetail.PurchaseItems);

            var item = purchaseDetail.PurchaseItems.First();
            Assert.Equal(_deviceWithStock.Id, item.DeviceId);
            Assert.Equal("NVIDIA", item.Brand);
            Assert.Equal("NVIDIA GeForce RTX 5090", item.Model);
            Assert.Equal("Negro", item.Color);
            Assert.Equal(999.99m, item.Price);
            Assert.Equal(2, item.Quantity);

            Assert.Single(_context.Purchases);
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
            var result = await controller.CreatePurchase(null!);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var validationProblem = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);

            Assert.Contains("Purchase", validationProblem.Errors.Keys);
            Assert.Contains(
                "La compra no puede ser nula",
                validationProblem.Errors["Purchase"]);
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task CreatePurchase_DeviceNotFound_ReturnsBadRequest()
        {
            var controller = new PurchasesController(
                _context,
                new Mock<ILogger<PurchasesController>>().Object);

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

            var result = await controller.CreatePurchase(purchaseForCreate);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var validationProblem = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);

            Assert.Contains("Device", validationProblem.Errors.Keys);
            Assert.Contains(
                "El dispositivo con id 999 no existe",
                validationProblem.Errors["Device"]);

            Assert.Empty(_context.Purchases);
            Assert.Empty(_context.PurchaseItems);
        }

        public static IEnumerable<object[]> TestCasesFor_CreatePurchase_BadRequest()
        {
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
                            4,
                            "Huawei_1",
                            "Huawei 2026_1",
                            "Plata",
                            799.99m,
                            3,
                            "Descripcion")
                    }),
                "Model"
           };

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
                            6,
                            "Xiaomi_1",
                            "Xiaomi 2026_1",
                            "Plata",
                            799.99m,
                            3,
                            "Descripcion")
                    }),
                "Model"
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
            var controller = new PurchasesController(
                _context,
                new Mock<ILogger<PurchasesController>>().Object);

            var result = await controller.CreatePurchase(purchaseForCreate);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var validationProblem = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);

            Assert.Contains(expectedErrorKey, validationProblem.Errors.Keys);
        }
    }
}