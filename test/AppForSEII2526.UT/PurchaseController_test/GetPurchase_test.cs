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
            var model = new Model(1, "NVIDIA GeForce RTX 5090");

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

            var user = new ApplicationUser("1", "Maria", "Torres", "maria@uclm.es")
            {
                UserName = "maria@uclm.es",
                Email = "maria@uclm.es"
            };

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

            var purchaseItem = new PurchaseItem(
                device,
                999.99m,
                2,
                _purchase)
            {
                Description = device.Description
            };

            _purchase.Items.Add(purchaseItem);
            _purchase.TotalPrice = 1999.98m;
            _purchase.TotalQuantity = 2;

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
            var controller = new PurchasesController(
                _context,
                new Mock<ILogger<PurchasesController>>().Object);

            var result = await controller.GetPurchase(_purchase.Id);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var purchaseDetail = Assert.IsType<PurchaseDetailDTO>(okResult.Value);

            Assert.Equal(_purchase.Id, purchaseDetail.Id);
            Assert.Equal("Maria", purchaseDetail.Name);
            Assert.Equal("Torres", purchaseDetail.Surname);
            Assert.Equal("Albacete", purchaseDetail.DeliveryAddress);
            Assert.Equal(_purchase.PurchaseDateUtc, purchaseDetail.PurchaseDateUtc);
            Assert.Equal(1999.98, purchaseDetail.TotalPrice, 2);
            Assert.Equal(2, purchaseDetail.TotalQuantity);

            Assert.Single(purchaseDetail.PurchaseItems);

            var item = purchaseDetail.PurchaseItems.First();
            Assert.Equal(1, item.DeviceId);
            Assert.Equal("NVIDIA", item.Brand);
            Assert.Equal("NVIDIA GeForce RTX 5090", item.Model);
            Assert.Equal("Negro", item.Color);
            Assert.Equal(999.99m, item.Price);
            Assert.Equal(2, item.Quantity);
            Assert.Equal("Máximo rendimiento en gaming 8K y tareas de IA", item.Description);
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetPurchase_NotFound_test()
        {
            var controller = new PurchasesController(
                _context,
                new Mock<ILogger<PurchasesController>>().Object);

            var result = await controller.GetPurchase(999);

            Assert.IsType<NotFoundResult>(result.Result);
        }
    }
}