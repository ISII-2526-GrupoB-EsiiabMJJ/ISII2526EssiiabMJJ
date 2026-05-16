using AppForMovies.UT;
using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.RepairDTOs;
using AppForSEII2526.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AppForSEII2526.UT.ReceiptsController_test
{
    public class PostReceipt_test : AppForSEII25264SqliteUT
    {
        private readonly ApplicationUser _user;
        private readonly Repair _screenRepair;
        private readonly Repair _batteryRepair;

        public PostReceipt_test()
        {
            var basicScale = new Scale("Básica") { Id = 1 };
            var mediumScale = new Scale("Media") { Id = 2 };

            _screenRepair = new Repair("Cambio de pantalla", "Sustitución de pantalla rota", 89.99m, basicScale.Id)
            {
                Id = 1,
                Scale = basicScale
            };

            _batteryRepair = new Repair("Cambio de batería", "Sustitución de batería degradada", 49.99m, mediumScale.Id)
            {
                Id = 2,
                Scale = mediumScale
            };

            _user = new ApplicationUser("1", "Maria", "Torres", "maria@uclm.es")
            {
                UserName = "maria@uclm.es",
                Email = "maria@uclm.es"
            };

            _context.Add(_user);
            _context.AddRange(basicScale, mediumScale);
            _context.AddRange(_screenRepair, _batteryRepair);
            _context.SaveChanges();
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task CreateReceipt_OK_test()
        {
            var controller = new ReceiptsController(
                _context,
                new Mock<ILogger<ReceiptsController>>().Object);

            var receiptForCreate = new ReceiptForCreateDTO(
                "maria@uclm.es",
                "Maria Torres",
                "Albacete",
                PaymentMethodTypes.CreditCard,
                new List<ReceiptItemForCreateDTO>
                {
                    new ReceiptItemForCreateDTO("iPhone 13", _screenRepair.Id),
                    new ReceiptItemForCreateDTO("Samsung S22", _batteryRepair.Id)
                });

            var result = await controller.CreateReceipt(receiptForCreate);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(ReceiptsController.GetReceipt), createdResult.ActionName);

            var receiptDetail = Assert.IsType<ReceiptDetailDTO>(createdResult.Value);

            Assert.Equal("Maria Torres", receiptDetail.CustomerNameSurname);
            Assert.Equal("Albacete", receiptDetail.DeliveryAddress);
            Assert.Equal(PaymentMethodTypes.CreditCard, receiptDetail.PaymentMethod);
            Assert.Equal(139.98m, receiptDetail.TotalPrice);
            Assert.Equal(2, receiptDetail.ReceiptItems.Count);

            Assert.Contains(receiptDetail.ReceiptItems, item =>
                item.Model == "iPhone 13" &&
                item.RepairName == "Cambio de pantalla" &&
                item.RepairCost == 89.99m &&
                item.Scale == "Básica");

            Assert.Contains(receiptDetail.ReceiptItems, item =>
                item.Model == "Samsung S22" &&
                item.RepairName == "Cambio de batería" &&
                item.RepairCost == 49.99m &&
                item.Scale == "Media");

            Assert.Single(_context.Receipts);
            Assert.Equal(2, _context.ReceiptItems.Count());
        }

        public static IEnumerable<object[]> TestCasesFor_CreateReceipt_BadRequest()
        {
            yield return new object[]
            {
                new ReceiptForCreateDTO(
                    "maria@uclm.es",
                    "Maria Torres",
                    "Albacete",
                    PaymentMethodTypes.CreditCard,
                    new List<ReceiptItemForCreateDTO>()),
                "ReceiptItems"
            };

            yield return new object[]
            {
                new ReceiptForCreateDTO(
                    "noexiste@uclm.es",
                    "Maria Torres",
                    "Albacete",
                    PaymentMethodTypes.CreditCard,
                    new List<ReceiptItemForCreateDTO>
                    {
                        new ReceiptItemForCreateDTO("iPhone 13", 1)
                    }),
                "ReceiptApplicationUser"
            };

            yield return new object[]
            {
                new ReceiptForCreateDTO(
                    "maria@uclm.es",
                    "Maria Torres",
                    "",
                    PaymentMethodTypes.CreditCard,
                    new List<ReceiptItemForCreateDTO>
                    {
                        new ReceiptItemForCreateDTO("iPhone 13", 1)
                    }),
                "DeliveryAddress"
            };

            yield return new object[]
            {
                new ReceiptForCreateDTO(
                    "maria@uclm.es",
                    "Maria Torres",
                    "Albacete",
                    PaymentMethodTypes.CreditCard,
                    new List<ReceiptItemForCreateDTO>
                    {
                        new ReceiptItemForCreateDTO("", 1)
                    }),
                "Model"
            };

            yield return new object[]
            {
                new ReceiptForCreateDTO(
                    "maria@uclm.es",
                    "Maria Torres",
                    "Albacete",
                    PaymentMethodTypes.CreditCard,
                    new List<ReceiptItemForCreateDTO>
                    {
                        new ReceiptItemForCreateDTO("iPhone 13", 999)
                    }),
                "Repair"
            };

            yield return new object[]
            {
                new ReceiptForCreateDTO(
                    "maria@uclm.es",
                    "Maria Torres",
                    "Albacete",
                    (PaymentMethodTypes)99,
                    new List<ReceiptItemForCreateDTO>
                    {
                        new ReceiptItemForCreateDTO("iPhone 13", 1)
                    }),
                "PaymentMethod"
            };
        }

        [Theory]
        [MemberData(nameof(TestCasesFor_CreateReceipt_BadRequest))]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task CreateReceipt_BadRequest_test(
            ReceiptForCreateDTO receiptForCreate,
            string expectedErrorKey)
        {
            var controller = new ReceiptsController(
                _context,
                new Mock<ILogger<ReceiptsController>>().Object);

            var result = await controller.CreateReceipt(receiptForCreate);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var validationProblem = Assert.IsType<ValidationProblemDetails>(badRequestResult.Value);

            Assert.Contains(expectedErrorKey, validationProblem.Errors.Keys);
        }
    }
}