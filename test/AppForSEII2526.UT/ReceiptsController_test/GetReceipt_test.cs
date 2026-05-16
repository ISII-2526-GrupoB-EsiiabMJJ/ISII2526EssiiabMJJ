using AppForMovies.UT;
using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.RepairDTOs;
using AppForSEII2526.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AppForSEII2526.UT.ReceiptsController_test
{
    public class GetReceipt_test : AppForSEII25264SqliteUT
    {
        private readonly Receipt _receipt;
        private readonly Repair _repair;

        public GetReceipt_test()
        {
            var scale = new Scale("Básica") { Id = 1 };

            _repair = new Repair("Cambio de pantalla", "Sustitución de pantalla rota", 89.99m, scale.Id)
            {
                Id = 1,
                Scale = scale
            };

            var user = new ApplicationUser("1", "Maria", "Torres", "maria@uclm.es")
            {
                UserName = "maria@uclm.es",
                Email = "maria@uclm.es"
            };

            _context.Add(user);
            _context.Add(scale);
            _context.Add(_repair);
            _context.SaveChanges();

            _receipt = new Receipt(
                "Maria Torres",
                "Albacete",
                PaymentMethodTypes.PayPal,
                new DateTime(2026, 5, 14, 10, 0, 0, DateTimeKind.Utc),
                89.99m,
                user);

            var receiptItem = new ReceiptItem
            {
                Model = "iPhone 13",
                RepairId = _repair.Id,
                Repair = _repair,
                Receipt = _receipt
            };

            _receipt.ReceiptItems.Add(receiptItem);

            _context.Add(_receipt);
            _context.SaveChanges();
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetReceipt_OK_test()
        {
            var controller = new ReceiptsController(
                _context,
                new Mock<ILogger<ReceiptsController>>().Object);

            var result = await controller.GetReceipt(_receipt.Id);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var receiptDetail = Assert.IsType<ReceiptDetailDTO>(okResult.Value);

            Assert.Equal(_receipt.Id, receiptDetail.Id);
            Assert.Equal("Maria Torres", receiptDetail.CustomerNameSurname);
            Assert.Equal("Albacete", receiptDetail.DeliveryAddress);
            Assert.Equal(PaymentMethodTypes.PayPal, receiptDetail.PaymentMethod);
            Assert.Equal(89.99m, receiptDetail.TotalPrice);
            Assert.Single(receiptDetail.ReceiptItems);

            var item = receiptDetail.ReceiptItems.First();

            Assert.Equal("iPhone 13", item.Model);
            Assert.Equal(_repair.Id, item.RepairId);
            Assert.Equal("Cambio de pantalla", item.RepairName);
            Assert.Equal("Sustitución de pantalla rota", item.RepairDescription);
            Assert.Equal(89.99m, item.RepairCost);
            Assert.Equal("Básica", item.Scale);
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetReceipt_NotFound_test()
        {
            var controller = new ReceiptsController(
                _context,
                new Mock<ILogger<ReceiptsController>>().Object);

            var result = await controller.GetReceipt(999);

            Assert.IsType<NotFoundResult>(result.Result);
        }
    }
}