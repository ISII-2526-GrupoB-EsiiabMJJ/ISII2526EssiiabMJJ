using AppForMovies.UT;
using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.RentalDTOSs;
using AppForSEII2526.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AppForSEII2526.UT.RentalsController_test
{
    public class GetRentals_test : AppForSEII25264SqliteUT
    {
        // Constructor para inicializar la base de datos de prueba
        public GetRentals_test()
        {
            var models = new List<Model>()
            {
                new Model(1,"NVIDIA GeForce RTX 5090"),
                new Model(2,"NVIDIA GeForce RTX 5080"),
                new Model(3,"NVIDIA GeForce RTX 4080 Ti"),
                new Model(1003,"NVIDIA GeForce RTX 5070")
            };

            var devices = new List<Device>()
            {
                new Device(1, 2025, QualityType.Hight, 3, 5, 59.99, 2199.99, "RTX 5090 Founders Edition","Negro","NVIDIA","Máximo rendimiento en gaming 8K y tareas de IA", models[0], new List<RentDevice>(), new List<ReviewItem>(), new List<PurchaseItem>()),
                new Device(2, 2025, QualityType.Medium, 8, 22, 49.99, 1699.99, "RTX 5080 Gaming Pro","Plata","MSI","Ideal para gaming 4K con DLSS 4.0", models[1], new List<RentDevice>(), new List<ReviewItem>(), new List<PurchaseItem>()),
                new Device(3, 2025, QualityType.Low, 6, 12, 39.99, 1299.99, "RTX 4080 Ti Dual Fan","Blanco","Gigabyte","Excelente rendimiento en 1440p y eficiencia térmica", models[2], new List<RentDevice>(), new List<ReviewItem>(), new List<PurchaseItem>()),
                new Device(4, 2025, QualityType.MediumHight, 5, 10, 64.99, 2299.99, "RTX 5090 OC Edition","Negro/Rojo","ASUS","Versión overclockeada con triple ventilador", models[0], new List<RentDevice>(), new List<ReviewItem>(), new List<PurchaseItem>())
            };

            var user = new ApplicationUser("1", "Petru", "Vlad", "petruvlad@uclm.es");

            var rentDevices = new List<RentDevice>();
            // Se añaden los primeros 2 dispositivos como ítems de alquiler al setup
            foreach (var device in devices.Take(2))
            {
                var rd = new RentDevice
                {
                    Device = device,
                    DeviceId = device.Id,
                    Price = device.priceForRent,
                    Quantity = 1
                };
                rentDevices.Add(rd);
            }

            var rental = new Rental(
                name: "petruvlad@uclm.es",
                surname: "Petru Vlad",
                applicationUser: user,
                deliveryAddress: "Albacete",
                rentalDate: DateTime.Parse("2025-11-01"),
                paymentMethod: PaymentMethodTypes.CreditCard,
                rentalDateFrom: DateTime.Parse("2025-11-01"),
                rentalDateTo: DateTime.Parse("2025-11-04"),
                rentalItems: rentDevices
            );

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

        // Prueba para verificar que la búsqueda de un alquiler inexistente retorna NotFound (404)
        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetRental_NotFound_test()
        {
            var mock = new Mock<ILogger<RentalsController>>();
            ILogger<RentalsController> logger = mock.Object;

            var controller = new RentalsController(_context, logger);
            var result = await controller.GetRental(0); // ID 0 no existe
            Assert.IsType<NotFoundResult>(result);
        }

        // Prueba para verificar que la búsqueda de un alquiler existente retorna Ok (200) con los datos correctos
        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task GetRental_Found_test()
        {
            var mock = new Mock<ILogger<RentalsController>>();
            ILogger<RentalsController> logger = mock.Object;

            var controller = new RentalsController(_context, logger);

            // Objeto DTO esperado con la corrección de 2 ítems de alquiler
            var expectedRental = new RentalDetailDTO(
                1,
                DateTime.Parse("2025-11-01"),
                "petruvlad@uclm.es",
                "Petru Vlad",
                "Albacete",
                PaymentMethodTypes.CreditCard,
                DateTime.Parse("2025-11-01"),
                DateTime.Parse("2025-11-04"),
                new List<RentalItemDTO>()
            );

            // Primer ítem esperado
            expectedRental.RentalItems.Add(new RentalItemDTO(
                1,
                "NVIDIA GeForce RTX 5090",
                59.99
            ));

            // ⭐ CORRECCIÓN: Agregar el segundo ítem, ya que el setup de la DB tiene 2 ítems.
            expectedRental.RentalItems.Add(new RentalItemDTO(
                2,
                "NVIDIA GeForce RTX 5080",
                49.99
            ));
            // ⭐ Ahora expectedRental.RentalItems.Count es 2.

            var result = await controller.GetRental(1);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var actualRental = Assert.IsType<RentalDetailDTO>(okResult.Value);

            // Aserciones de las propiedades del alquiler
            Assert.Equal(expectedRental.Id, actualRental.Id);
            Assert.Equal(expectedRental.RentalDate, actualRental.RentalDate);
            Assert.Equal(expectedRental.CustomerUserName, actualRental.CustomerUserName);
            Assert.Equal(expectedRental.CustomerNameSurname, actualRental.CustomerNameSurname);
            Assert.Equal(expectedRental.DeliveryAddress, actualRental.DeliveryAddress);
            Assert.Equal(expectedRental.PaymentMethod, actualRental.PaymentMethod);
            Assert.Equal(expectedRental.RentalDateFrom, actualRental.RentalDateFrom);
            Assert.Equal(expectedRental.RentalDateTo, actualRental.RentalDateTo);

            // Aserción del conteo de ítems (donde ocurría el error)
            Assert.Equal(expectedRental.RentalItems.Count, actualRental.RentalItems.Count);

            // Aserciones de las propiedades de los ítems
            for (int i = 0; i < expectedRental.RentalItems.Count; i++)
            {
                var expectedItem = expectedRental.RentalItems[i];
                var actualItem = actualRental.RentalItems[i];

                Assert.Equal(expectedItem.DeviceID, actualItem.DeviceID);
                Assert.Equal(expectedItem.Model, actualItem.Model);
                Assert.Equal(expectedItem.PriceForRent, actualItem.PriceForRent);
                // Nota: la aserción de Description está comentada porque no se incluyó en la definición de RentalItemDTO
                // Assert.Equal(expectedItem.Description, actualItem.Description); 
            }
        }
    }
}