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
    public class GetRepairsForReceipt_test : AppForSEII25264SqliteUT
    {
        public GetRepairsForReceipt_test()
        {
            var basicScale = new Scale("Básica") { Id = 1 };
            var mediumScale = new Scale("Media") { Id = 2 };
            var luxuryScale = new Scale("Lujo") { Id = 3 };

            var repairs = new List<Repair>
            {
                new Repair("Cambio de pantalla", "Sustitución de pantalla rota", 89.99m, basicScale.Id)
                {
                    Id = 1,
                    Scale = basicScale
                },
                new Repair("Cambio de batería", "Sustitución de batería degradada", 49.99m, mediumScale.Id)
                {
                    Id = 2,
                    Scale = mediumScale
                },
                new Repair("Reparación placa base", "Reparación avanzada de placa base", 149.99m, luxuryScale.Id)
                {
                    Id = 3,
                    Scale = luxuryScale
                }
            };

            _context.AddRange(basicScale, mediumScale, luxuryScale);
            _context.AddRange(repairs);
            _context.SaveChanges();
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetRepairsForReceipt_OK_test()
        {
            var controller = new ReceiptsController(
                _context,
                new Mock<ILogger<ReceiptsController>>().Object);

            var result = await controller.GetRepairsForReceipt(null, null);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var repairs = Assert.IsAssignableFrom<IList<RepairForReceiptDTO>>(okResult.Value);

            Assert.Equal(3, repairs.Count);
            Assert.Contains(repairs, r => r.Name == "Cambio de pantalla");
            Assert.Contains(repairs, r => r.Name == "Cambio de batería");
            Assert.Contains(repairs, r => r.Name == "Reparación placa base");
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetRepairsForReceipt_FilterByName_OK_test()
        {
            var controller = new ReceiptsController(
                _context,
                new Mock<ILogger<ReceiptsController>>().Object);

            var result = await controller.GetRepairsForReceipt("pantalla", null);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var repairs = Assert.IsAssignableFrom<IList<RepairForReceiptDTO>>(okResult.Value);

            Assert.Single(repairs);
            Assert.Equal("Cambio de pantalla", repairs.First().Name);
            Assert.Equal("Básica", repairs.First().Scale);
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetRepairsForReceipt_FilterByScale_OK_test()
        {
            var controller = new ReceiptsController(
                _context,
                new Mock<ILogger<ReceiptsController>>().Object);

            var result = await controller.GetRepairsForReceipt(null, "lujo");

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var repairs = Assert.IsAssignableFrom<IList<RepairForReceiptDTO>>(okResult.Value);

            Assert.Single(repairs);
            Assert.Equal("Reparación placa base", repairs.First().Name);
            Assert.Equal("Lujo", repairs.First().Scale);
        }

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetRepairsForReceipt_NoResults_OK_test()
        {
            var controller = new ReceiptsController(
                _context,
                new Mock<ILogger<ReceiptsController>>().Object);

            var result = await controller.GetRepairsForReceipt("altavoz", null);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var repairs = Assert.IsAssignableFrom<IList<RepairForReceiptDTO>>(okResult.Value);

            Assert.Empty(repairs);
        }
    }
}