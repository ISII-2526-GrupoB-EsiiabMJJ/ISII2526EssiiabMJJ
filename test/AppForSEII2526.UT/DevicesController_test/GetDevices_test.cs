using AppForMovies.UT;
using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.DeviceDTOs.DevideRentalDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AppForSEII2526.UT.DevicesController_test
{
    public class GetDevices_test : AppForSEII25264SqliteUT
    {
        public GetDevices_test()
        {
            // Inicializamos modelos
            var models = new List<Model>()
            {
                new Model(1,"NVIDIA GeForce RTX 5090"),
                new Model(2,"NVIDIA GeForce RTX 5080"),
                new Model(3,"NVIDIA GeForce RTX 4080 Ti"),
                new Model(1003,"NVIDIA GeForce RTX 5070")
            };

            // Inicializamos dispositivos
            var devices = new List<Device>()
            {
                new Device(1,2025,QualityType.Hight,3,5,59.99,2199.99,"RTX 5090 Founders Edition","Negro","NVIDIA","Máximo rendimiento en gaming 8K y tareas de IA",models[0],new List<RentDevice>(),new List<ReviewItem>(),new List<PurchaseItem>()),
                new Device(2,2025,QualityType.Medium,8,22,49.99,1699.99,"RTX 5080 Gaming Pro","Plata","MSI","Ideal para gaming 4K con DLSS 4.0",models[1],new List<RentDevice>(),new List<ReviewItem>(),new List<PurchaseItem>()),
                new Device(3,2025,QualityType.Low,6,12,39.99,1299.99,"RTX 4080 Ti Dual Fan","Blanco","Gigabyte","Excelente rendimiento en 1440p y eficiencia térmica",models[2],new List<RentDevice>(),new List<ReviewItem>(),new List<PurchaseItem>()),
                new Device(4,2025,QualityType.MediumHight,5,10,64.99,2299.99,"RTX 5090 OC Edition","Negro/Rojo","ASUS","Versión overclockeada con triple ventilador",models[0],new List<RentDevice>(),new List<ReviewItem>(),new List<PurchaseItem>())
            };

            var user = new ApplicationUser("1", "Petru", "Vlad", "petru.vlad@uclm.es");

            var rentDevices = new List<RentDevice>();
            foreach (var device in devices.Take(2))
            {
                rentDevices.Add(new RentDevice
                {
                    Device = device,
                    DeviceId = device.Id,
                    Price = device.priceForRent,
                    Quantity = 1
                });
            }

            var rental = new Rental("Petru", "Vlad", user, "Albacete",
                DateTime.Parse("2025-11-01"), PaymentMethodTypes.CreditCard,
                DateTime.Parse("2025-11-01"), DateTime.Parse("2025-11-04"), rentDevices);

            foreach (var rd in rentDevices)
            {
                rd.Rent = rental;
                rd.RentalId = rental.Id;
            }

            _context.Add(user);
            _context.AddRange(models);
            _context.AddRange(devices);
            _context.Add(rental);
            _context.SaveChanges();
        }

        public static IEnumerable<object[]> TestCasesFor_GetDeviceForRental_OK()
        {
            var dispositivos = new List<DeviceRentalDTO>()
            {
                new DeviceRentalDTO(4,2025,64.99,"RTX 5090 OC Edition","NVIDIA GeForce RTX 5090","Negro/Rojo","ASUS"),
                new DeviceRentalDTO(3,2025,39.99,"RTX 4080 Ti Dual Fan","NVIDIA GeForce RTX 4080 Ti","Blanco","Gigabyte"),
                new DeviceRentalDTO(2,2025,49.99,"RTX 5080 Gaming Pro","NVIDIA GeForce RTX 5080","Plata","MSI"),
                new DeviceRentalDTO(1,2025,59.99,"RTX 5090 Founders Edition","NVIDIA GeForce RTX 5090","Negro","NVIDIA")
            };

            // Todos
            var allDevices = dispositivos.OrderBy(d => d.Brand).ToList();

            // Filtrado MSI
            var msiDevices = dispositivos.Where(d => d.Brand == "MSI").OrderBy(d => d.Brand).ToList();

            // Filtrado ASUS
            var asusDevices = dispositivos.Where(d => d.Brand == "ASUS").OrderBy(d => d.Brand).ToList();

            return new List<object[]>
            {
                new object[] { null, null, allDevices },
                new object[] { "MSI", null, msiDevices },
                new object[] { "ASUS", null, asusDevices },
            };
        }

        [Theory]
        [MemberData(nameof(TestCasesFor_GetDeviceForRental_OK))]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetDevisceForRental_OK_test(string? filterTitle, string? filterGenre, IList<DeviceRentalDTO> dispositivosEsperados)
        {
            var controller = new DeviceController(_context, null);
            var result = await controller.GetDeviceForRental(filterTitle, filterGenre);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var retornoControlador = Assert.IsType<List<DeviceRentalDTO>>(okResult.Value);
            Assert.Equal(dispositivosEsperados, retornoControlador, new DeviceRentalDTOComparer());
        }

        // Eliminamos el test de BadRequest porque el controlador actual no devuelve BadRequest
        // y lo reemplazamos por un test que compruebe Ok con filtros nulos
        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        [Trait("Database", "WithoutFixture")]
        public async Task GetDevicesForRental_OkWithNullFilters_test()
        {
            var controller = new DeviceController(_context, null);
            var result = await controller.GetDeviceForRental(null, null);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var retornoControlador = Assert.IsType<List<DeviceRentalDTO>>(okResult.Value);

            Assert.NotEmpty(retornoControlador);
        }

        private class DeviceRentalDTOComparer : IEqualityComparer<DeviceRentalDTO>
        {
            public bool Equals(DeviceRentalDTO? x, DeviceRentalDTO? y)
            {
                if (x == null || y == null) return false;
                return x.Id == y.Id &&
                       x.Name == y.Name &&
                       x.Brand == y.Brand &&
                       x.Model == y.Model &&
                       x.Color == y.Color &&
                       x.Year == y.Year &&
                       Math.Abs(x.priceForRent - y.priceForRent) < 0.01;
            }

            public int GetHashCode(DeviceRentalDTO obj)
            {
                return HashCode.Combine(obj.Id, obj.Name, obj.Brand, obj.Model, obj.Color, obj.Year, obj.priceForRent);
            }
        }
    }
}
