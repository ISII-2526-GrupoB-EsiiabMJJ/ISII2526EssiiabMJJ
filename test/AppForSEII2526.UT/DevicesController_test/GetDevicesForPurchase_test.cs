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
            // Modelos asociados a los dispositivos que se usarán en las pruebas de búsqueda.
            var models = new List<Model>()
            {
                new Model(1, "NVIDIA GeForce RTX 5090"),
                new Model(2, "NVIDIA GeForce RTX 5080"),
                new Model(3, "NVIDIA GeForce RTX 4080 Ti"),
                new Model(4, "AMD Radeon RX 7900")
            };

            // Dispositivos disponibles en la base de datos de pruebas.
            // Se incluyen distintos nombres, colores, marcas y cantidades para comprobar los filtros.
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

                // Este dispositivo tiene cantidad de compra 0, por lo que no debe aparecer
                // en el listado de dispositivos disponibles para compra.
                new Device(4, 2025, QualityType.MediumHight, 5, 0, 64.99, 2299.99,
                    "RTX 5090 OC Edition", "Negro/Rojo", "ASUS",
                    "Versión overclockeada con triple ventilador",
                    models[0], new List<RentDevice>(), new List<ReviewItem>(), new List<PurchaseItem>()),

                new Device(5, 2025, QualityType.Medium, 4, 7, 44.99, 1199.99,
                    "Radeon RX 7900 XT", "Negro", "AMD",
                    "GPU AMD de alto rendimiento",
                    models[3], new List<RentDevice>(), new List<ReviewItem>(), new List<PurchaseItem>())
            };

            // Se guardan los datos necesarios para que el controlador pueda aplicar los filtros.
            _context.AddRange(models);
            _context.AddRange(devices);
            _context.SaveChanges();
        }

        public static IEnumerable<object[]> TestCasesFor_GetDevicesForPurchase_OK()
        {
            // Resultado base esperado cuando no se aplican filtros.
            // Solo se incluyen dispositivos con stock de compra mayor que 0.
            var dispositivos = new List<DeviceForPurchaseDTO>()
            {
                new DeviceForPurchaseDTO(
                    3,
                    "RTX 4080 Ti Dual Fan",
                    1299.99,
                    "Gigabyte",
                    "NVIDIA GeForce RTX 4080 Ti",
                    "Blanco"),

                new DeviceForPurchaseDTO(
                    2,
                    "RTX 5080 Gaming Pro",
                    1699.99,
                    "MSI",
                    "NVIDIA GeForce RTX 5080",
                    "Plata"),

                new DeviceForPurchaseDTO(
                    1,
                    "RTX 5090 Founders Edition",
                    2199.99,
                    "NVIDIA",
                    "NVIDIA GeForce RTX 5090",
                    "Negro"),

                new DeviceForPurchaseDTO(
                    5,
                    "Radeon RX 7900 XT",
                    1199.99,
                    "AMD",
                    "AMD Radeon RX 7900",
                    "Negro")
            };

            return new List<object[]>
            {
                // Caso 1: sin filtros.
                // Debe devolver todos los dispositivos disponibles para compra.
                new object[]
                {
                    null,
                    null,
                    dispositivos
                },

                // Caso 2: filtro por nombre.
                // Debe devolver solo los dispositivos cuyo nombre contiene el texto indicado.
                new object[]
                {
                    "5090",
                    null,
                    dispositivos.Where(d => d.Name.Contains("5090")).ToList()
                },

                // Caso 3: filtro por color.
                // Debe devolver los dispositivos cuyo color contiene el texto indicado.
                new object[]
                {
                    null,
                    "negro",
                    dispositivos.Where(d => d.Color.ToLower().Contains("negro")).ToList()
                },

                // Caso 4: filtros combinados.
                // El dispositivo debe cumplir simultáneamente el filtro de nombre y el de color.
                new object[]
                {
                    "RTX",
                    "blanco",
                    dispositivos
                        .Where(d => d.Name.Contains("RTX") &&
                                    d.Color.ToLower().Contains("blanco"))
                        .ToList()
                },

                // Caso 5: búsqueda sin resultados.
                // El controlador debe responder OK con una lista vacía.
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
            // Arrange
            // Se instancia el controlador con el contexto que contiene los dispositivos de prueba.
            var logger = new Mock<ILogger<DeviceController>>();
            var controller = new DeviceController(_context, logger.Object);

            // Act
            // Se ejecuta la consulta de dispositivos para compra con los filtros recibidos.
            var result = await controller.GetDevicesForPurchase(name, color);

            // Assert
            // La búsqueda debe devolver OK tanto si hay resultados como si la lista queda vacía.
            var okResult = Assert.IsType<OkObjectResult>(result.Result);

            // El valor devuelto debe ser una lista de DTOs preparada para la pantalla de compra.
            var retornoControlador = Assert.IsType<List<DeviceForPurchaseDTO>>(okResult.Value);

            // Se compara la lista esperada con la devuelta por el controlador.
            // El comparador evita depender de la referencia de los objetos y compara campo a campo.
            Assert.Equal(
                dispositivosEsperados,
                retornoControlador,
                new DeviceForPurchaseDTOComparer());
        }

        private class DeviceForPurchaseDTOComparer : IEqualityComparer<DeviceForPurchaseDTO>
        {
            public bool Equals(DeviceForPurchaseDTO? x, DeviceForPurchaseDTO? y)
            {
                // Si alguno de los objetos es nulo, no se consideran iguales.
                if (x == null || y == null)
                    return false;

                // Se comparan todos los campos relevantes que debe mostrar la pantalla de compra.
                return x.Id == y.Id &&
                       x.Name == y.Name &&
                       Math.Abs(x.Price - y.Price) < 0.01 &&
                       x.Brand == y.Brand &&
                       x.Model == y.Model &&
                       x.Color == y.Color;
            }

            public int GetHashCode(DeviceForPurchaseDTO obj)
            {
                // Se genera el hash con los mismos campos usados en la comparación.
                return HashCode.Combine(
                    obj.Id,
                    obj.Name,
                    obj.Price,
                    obj.Brand,
                    obj.Model,
                    obj.Color);
            }
        }
    }
}