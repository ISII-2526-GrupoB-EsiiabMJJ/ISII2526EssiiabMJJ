using AppForMovies.UT;
using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.RentalDTOSs;
using AppForSEII2526.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AppForSEII2526.UT.RentalsController_test
{
    public class PostRentals_test : AppForSEII25264SqliteUT
    {
        private const string _userName = "petru.vlad@uclm.es";
        private const string _customerNameSurname = "Petru Vlad";
        private const string _deliveryAddress = "Calle Luna 45";

        private const string _device1Name = "RTX 5090 Founders Edition";
        private const string _device2Name = "RTX 5080 Gaming Pro";
        private const string _model1Name = "NVIDIA GeForce RTX 5090";
        private const string _model2Name = "NVIDIA GeForce RTX 5080";

        public PostRentals_test()
        {
            var models = new List<Model>()
            {
                new Model(1, _model1Name),
                new Model(2, _model2Name)
            };

            var devices = new List<Device>()
            {
                new Device(1, 2025, QualityType.Hight, 3, 5, 59.99, 2199.99, _device1Name, "Negro", "NVIDIA", "Máximo rendimiento en gaming 8K y tareas de IA", models[0], new List<RentDevice>(), new List<ReviewItem>(), new List<PurchaseItem>()),
                new Device(2, 2025, QualityType.Medium, 8, 22, 49.99, 1699.99, _device2Name, "Plata", "MSI", "Ideal para gaming 4K con DLSS 4.0", models[1], new List<RentDevice>(), new List<ReviewItem>(), new List<PurchaseItem>())
            };

            var user = new ApplicationUser("1", "Petru", "Vlad", _userName);

            var rentDevices = new List<RentDevice>();
            foreach (var device in devices)
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
                name: "Petru",
                surname: "Vlad",
                applicationUser: user,
                deliveryAddress: _deliveryAddress,
                rentalDate: DateTime.Now,
                paymentMethod: PaymentMethodTypes.CreditCard,
                rentalDateFrom: DateTime.Today.AddDays(2),
                rentalDateTo: DateTime.Today.AddDays(5),
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

        [Fact]
        [Trait("Database", "WithoutFixture")]
        [Trait("LevelTesting", "Unit Testing")]
        public async Task CreateRental_Success_test()
        {
            var mock = new Mock<ILogger<RentalsController>>();
            var logger = mock.Object;
            var controller = new RentalsController(_context, logger);

            /*int id, string customerUserName, string customerNameSurname, 
             * string deliveryAddress, PaymentMethodTypes paymentMethod, 
             * DateTime rentalDateFrom, DateTime rentalDateTo, IList<RentalItemDTO> rentalItems*/
            var rentalDTO = new RentalForCreateDTO(
               1,
                _userName,
                _customerNameSurname,
                _deliveryAddress,
                PaymentMethodTypes.CreditCard,
                DateTime.Today.AddDays(2),
                DateTime.Today.AddDays(5),
                new List<RentalItemDTO>()
                {
                    new RentalItemDTO(1, _model1Name, 59.99)
                }
            );

            var expectedRental = new RentalDetailDTO(
                2,
                DateTime.Now,
                _userName,
                _customerNameSurname,
                _deliveryAddress,
                PaymentMethodTypes.CreditCard,
                DateTime.Today.AddDays(2),
                DateTime.Today.AddDays(5),
                new List<RentalItemDTO>()
                {
                    new RentalItemDTO(1, _model1Name, 59.99)
                } 
            );

            var result = await controller.CreateRental(rentalDTO);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var actualRental = Assert.IsType<RentalDetailDTO>(createdResult.Value);

            Assert.Equal(expectedRental.CustomerUserName, actualRental.CustomerUserName);
            Assert.Equal(expectedRental.CustomerNameSurname, actualRental.CustomerNameSurname);
            Assert.Equal(expectedRental.DeliveryAddress, actualRental.DeliveryAddress);
            Assert.Equal(expectedRental.RentalItems.Count, actualRental.RentalItems.Count);
            Assert.Equal(expectedRental.RentalItems[0].DeviceID, actualRental.RentalItems[0].DeviceID);
            Assert.Equal(expectedRental.RentalItems[0].Model, actualRental.RentalItems[0].Model);
            Assert.Equal(expectedRental.RentalItems[0].PriceForRent, actualRental.RentalItems[0].PriceForRent);
        }
    }
}
