using AppForMovies.UT;
using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.DeviceDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AppForSEII2526.UT.DevicesController_test
{
    public class GetDevicesForPurchase_test : AppForSEII25264SqliteUT
    {
        public GetDevicesForPurchase_test()
        {
            var models = new List<Model>()
            {
                new Model(1, "NVIDIA GeForce RTX 5090"),
                new Model(2, "NVIDIA GeForce RTX 5080"),
                new Model(3, "NVIDIA GeForce RTX 4080 Ti"),
                new Model(4, "AMD Radeon RX 7900")
            };

            var devices = new List<Device>()
            {
                new Device(1, 2025, QualityType.Hight, 3, 5, 59.99, 2199.99,
                    "RTX 5090 Founders Edition", "Negro", "NVIDIA",
                    "Máximo rendimiento en gaming 8K y tareas de IA",
                    models[0], new List<RentDevice>(), new List<ReviewItem>(), new List<PurchaseItem>()),

                new Device(2, 2025, QualityType.Medium, 8, 22, 49.99, 1699.99,
                    "RTX 5080 Gaming Pro", "Plata", "MSI",
                    "Ideal para gaming 4K con DLSS 4.0",
                    models[1], new List<RentDevice>(), new List<ReviewItem>(), new List<PurchaseItem>()),

                new Device(3, 2025, QualityType.Low, 6, 12, 39.99, 1299.99,
                    "RTX 4080 Ti Dual Fan", "Blanco", "Gigabyte",
                    "Excelente rendimiento en 1440p y eficiencia térmica",
                    models[2], new List<RentDevice>(), new List<ReviewItem>(), new List<PurchaseItem>()),

                new Device(4, 2025, QualityType.MediumHight, 5, 0, 64.99, 2299.99,
                    "RTX 5090 OC Edition", "Negro/Rojo", "ASUS",
                    "Versión overclockeada con triple ventilador",
                    models[0], new List<RentDevice>(), new List<ReviewItem>(), new List<PurchaseItem>()),

                new Device(5, 2025, QualityType.Medium, 4, 7, 44.99, 1199.99,
                    "Radeon RX 7900 XT", "Negro", "AMD",
                    "GPU AMD de alto rendimiento",
                    models[3], new List<RentDevice>(), new List<ReviewItem>(), new List<PurchaseItem>())
            };

            _context.AddRange(models);
            _context.AddRange(devices);
            _context.SaveChanges();
        }

        public static IEnumerable<object[]> TestCasesFor_GetDevicesForPurchase_OK()
        {
            var dispositivos = new List<DeviceForPurchaseDTO>()
    {
        new DeviceForPurchaseDTO(3, "RTX 4080 Ti Dual Fan", 1299.99, "Gigabyte", "NVIDIA GeForce RTX 4080 Ti", "Blanco"),
        new DeviceForPurchaseDTO(2, "RTX 5080 Gaming Pro", 1699.99, "MSI", "NVIDIA GeForce RTX 5080", "Plata"),
        new DeviceForPurchaseDTO(1, "RTX 5090 Founders Edition", 2199.99, "NVIDIA", "NVIDIA GeForce RTX 5090", "Negro"),
        new DeviceForPurchaseDTO(5, "Radeon RX 7900 XT", 1199.99, "AMD", "AMD Radeon RX 7900", "Negro")
    };

            return new List<object[]>
    {
        // Sin filtros: devuelve todos los dispositivos con stock de compra > 0.
        new object[]
        {
            null,
            null,
            dispositivos
        },

        // Filtro por nombre.
        new object[]
        {
            "5090",
            null,
            dispositivos.Where(d => d.Name.Contains("5090")).ToList()
        },

        // Filtro por color.
        new object[]
        {
            null,
            "negro",
            dispositivos.Where(d => d.Color.ToLower().Contains("negro")).ToList()
        },

        // Filtros combinados.
        new object[]
        {
            "RTX",
            "blanco",
            dispositivos.Where(d => d.Name.Contains("RTX") && d.Color.ToLower().Contains("blanco")).ToList()
        },

        // Sin resultados.
        new object[]
        {
            "NoExiste",
            null,
            new List<DeviceForPurchaseDTO>()
        }
    };
        }

        [Theory]
        [MemberData(nameof(TestCasesFor_GetDevicesForPurchase_OK))]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetDevicesForPurchase_OK_test(
            string? name,
            string? color,
            IList<DeviceForPurchaseDTO> dispositivosEsperados)
        {
            var logger = new Mock<ILogger<DeviceController>>();
            var controller = new DeviceController(_context, logger.Object);

            var result = await controller.GetDevicesForPurchase(name, color);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var retornoControlador = Assert.IsType<List<DeviceForPurchaseDTO>>(okResult.Value);

            Assert.Equal(dispositivosEsperados, retornoControlador, new DeviceForPurchaseDTOComparer());
        }

        private class DeviceForPurchaseDTOComparer : IEqualityComparer<DeviceForPurchaseDTO>
        {
            public bool Equals(DeviceForPurchaseDTO? x, DeviceForPurchaseDTO? y)
            {
                if (x == null || y == null) return false;

                return x.Id == y.Id &&
                       x.Name == y.Name &&
                       Math.Abs(x.Price - y.Price) < 0.01 &&
                       x.Brand == y.Brand &&
                       x.Model == y.Model &&
                       x.Color == y.Color;
            }

            public int GetHashCode(DeviceForPurchaseDTO obj)
            {
                return HashCode.Combine(obj.Id, obj.Name, obj.Price, obj.Brand, obj.Model, obj.Color);
            }
        }
    }
}