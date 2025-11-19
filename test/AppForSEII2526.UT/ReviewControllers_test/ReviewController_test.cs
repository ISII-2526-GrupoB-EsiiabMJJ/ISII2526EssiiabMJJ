using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs.ReviewDTOs;
using AppForSEII2526.API.DTOs.UserDTOs;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Data.Common;
using Xunit.Abstractions;
using static NuGet.Packaging.PackagingConstants;

namespace AppForPCS.UT.ReparacionesController
{
    public class ReviewController_test
    {
        private readonly DbConnection _connection;
        private readonly DbContextOptions<ApplicationDbContext> _contextOptions;
        private readonly ITestOutputHelper _output;

        ApplicationDbContext CreateContext() => new(_contextOptions);
        void Dispose() => _connection.Dispose();

        public ReviewController_test(ITestOutputHelper output)
        {
            _output = output;
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            _contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(_connection).Options;

            using var context = new ApplicationDbContext(_contextOptions);

            if (context.Database.EnsureCreated())
            {
                using var viewCommand = context.Database.GetDbConnection().CreateCommand();
                viewCommand.CommandText = @"
                CREATE VIEW AllResources AS
                SELECT Name
                FROM Movies;";
                viewCommand.ExecuteNonQuery();
            }


            var usuarios = new List<ApplicationUser>()
            {
                new ApplicationUser("1", "Petru", "Vlad", "rumanoloKo"),
                new ApplicationUser("2", "Vlad", "Vladislav", "vladis"),
                new ApplicationUser("3", "Mihai", "Varcea", "varicia")
            };

            var modelos = new List<Model>()
            {
                new Model(
                    1,
                    "NVIDIA GeForce RTX 5090"),
                new Model(
                    2,
                    "NVIDIA GeForce RTX 5080"),
                new Model(
                    3,
                    "NVIDIA GeForce RTX 4080 Ti"),
                new Model(
                    4,
                    "NVIDIA GeForce RTX 4090"),
            };

            var dispositivos = new List<Device>()
            {
                new Device(
                    1,
                    2025,
                    QualityType.Hight,
                    5,
                    15,
                    59.99,
                    2199.99,
                    "RTX 5090 Founders Edition",
                    "Negro",
                    "NVIDIA",
                    "Máximo rendimiento en gaming 8K y tareas de IA",
                    modelos[0],
                    null,
                    null,
                    null),

                new Device(
                    2,
                    2025,
                    QualityType.MediumHight,
                    8,
                    22,
                    49.99,
                    1699.99,
                    "RTX 5080 Gaming Pro",
                    "Plata", "MSI",
                    "Ideal para gaming 4K con DLSS 4.0",
                    modelos[1],
                    null,
                    null,
                    null),
                new Device(
                    3,
                    2024,
                    QualityType.MediumHight,
                    6,
                    19,
                    39.99,
                    1299.99,
                    "RTX 4080 Ti Dual Fan",
                    "Blanco", "Gigabyte",
                    "Excelente rendimiento en 1440p y eficiencia térmica",
                    modelos[2],
                    null,
                    null,
                    null),
                new Device(4,
                    2025,
                    QualityType.Hight,
                    3,
                    12,
                    64.99,
                    2299.99,
                    "RTX 5090 OC Edition",
                    "Rojo",
                    "Gigabyte",
                    "Versión overclockeada con triple ventilador",
                    modelos[0],
                    null,
                    null,
                    null),
                new Device(5,
                    2024,
                    QualityType.MediumHight,
                    3,
                    12,
                    54.99,
                    1899.99,
                    "RTX 4090 Dual Ultimate",
                    "Negro",
                    "MSI",
                    "Excelente rendimiento en 4K y eficiencia persistente",
                    modelos[3],
                    null,
                    null,
                    null),
            };


            var reseñas = new List<Review>()
            {
                new Review(
                    1,
                    "1",
                    5,
                    "Máximo rendimiento en IA y gaming extremo",
                    "España",
                    null,
                    usuarios[0],
                    new DateTime(2025, 9, 10, 10, 0, 0),
                    null
                ),
                new Review(
                    2,
                    "2",
                    4,
                    "Gran potencia, pero requiere buena ventilación",
                    "Alemania",
                    null,
                    usuarios[1],
                    new DateTime(2025, 9, 11, 12, 30, 0),
                    null
                ),
                new Review(
                    3,
                    "3",
                    5,
                    "Silencio y eficiencia en largas sesiones",
                    "Francia",
                    null,
                    usuarios[2],
                    new DateTime(2025, 9, 12, 16, 45, 0),
                    null
                )
            };

            var elementos_de_reseñas = new List<ReviewItem>()
            {
                new ReviewItem(1, 5, "Potencia brutal, ideal para IA y gaming extremo", dispositivos[0], reseñas[0]),
                new ReviewItem(2, 4, "Requiere buena ventilación, pero rinde excelente", dispositivos[0], reseñas[1]),
                new ReviewItem(3, 5, "Silenciosa y eficiente en cargas largas", dispositivos[1], reseñas[2]),
                new ReviewItem(4, 4, "Buen rendimiento en 1440p, drivers estables", dispositivos[2], reseñas[1])
            };




            context.AddRange(usuarios);
            context.AddRange(modelos);
            context.AddRange(dispositivos);
            context.AddRange(reseñas);
            context.AddRange(elementos_de_reseñas);
            context.SaveChanges();
        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        public void GetReviewDetalles_Found_test()
        {
            // Arrange
            using var context = CreateContext();
            var mock = new Mock<ILogger<ReviewsController>>();
            ILogger<ReviewsController> logger = mock.Object;
            var controller = new ReviewsController(context, logger);

            var reseñaEsperada =
                new ReviewDetailsDTO(
                    1,
                    null,
                    "España",
                    "Máximo rendimiento en IA y gaming extremo",
                    new DateTime(2025, 9, 10, 10, 0, 0),
                    new List<ReviewDeviceDTO>());

            reseñaEsperada.ReviewDetailsDevicesList.Add(
                new ReviewDeviceDTO(
                    1,
                    "RTX 5090 Founders Edition",
                    "NVIDIA GeForce RTX 5090",
                    2025,
                    5,
                    "Potencia brutal, ideal para IA y gaming extremo")
            );

            // Act
            var result = controller.ReviewDetails(1);
            //Assert
            //we check that the response type is OK and obtain the list of movies
            var listaReseñasRecibidas = Assert.IsType<OkObjectResult>(result.Result.Result).Value;
            //we check that the expected and actual are the same
            Assert.Equal(listaReseñasRecibidas, reseñaEsperada);

        }

        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        public void GetReviewDetails_NotFound_test()
        {
            // Arrange
            var mock = new Mock<ILogger<ReviewsController>>();
            ILogger<ReviewsController> logger = mock.Object;

            using var context = CreateContext();
            var controller = new ReviewsController(context, logger);

            // Act
            var result = controller.ReviewDetails(0);

            //Assert
            Assert.IsType<NotFoundResult>(result.Result.Result);

        }

        public static IEnumerable<object[]> CaseTest_Error_PostReview()
        {
            //////////////////////////////////////////
            //////                              //////
            //////       Sin dispositivos       //////
            //////                              //////
            //////////////////////////////////////////
            CreateReviewDTO review_without_devices = new CreateReviewDTO(
                "1",
                "Review buena de un producto malo", 
                "Italy", 
                "Giorno",
                DateTime.Now, 
                new List<ReviewItemDTO>());
            //////////////////////////////////////////
            //////                              //////
            //////   Dispositivo inexistente    //////
            //////                              //////
            //////////////////////////////////////////
            CreateReviewDTO review_with_unexistent_devices = new CreateReviewDTO(
                    "1",
                    "Review buena de un producto malo",
                    "Italy",
                    "Giorno",
                    DateTime.Now,
                    new List<ReviewItemDTO>());
            ReviewItemDTO unexistent_devices = new ReviewItemDTO(
                -1,
                2025,5,
                "NVIDIA GeForce RTX 5090",
                "Asus",
                "Potencia brutal, ideal para IA y gaming extremo");
            review_with_unexistent_devices.ReviewItems.Add(unexistent_devices);


            //////////////////////////////////////////
            //////                              //////
            //////    ApplicationUser falso     //////
            //////                              //////
            //////////////////////////////////////////
            CreateReviewDTO review_without_real_customer = new CreateReviewDTO(
                "-1",
                "Review buena de un producto malo",
                "Italy",
                "Giorno",
                DateTime.Now,
                new List<ReviewItemDTO>());

            ReviewItemDTO good_device = new ReviewItemDTO(
                1,
                2025,
                5,
                "NVIDIA GeForce RTX 5090",
                "RTX 5090 Founders Edition",
                "Máximo rendimiento en gaming e IA");
            review_without_real_customer.ReviewItems.Add(good_device);


            var todosLosTests = new List<object[]>{
                new object[] { review_without_devices, "Debes incluir al menos un dispositivo reseñado.",  },
                new object[] { review_with_unexistent_devices, $"El dispositivo con ID {unexistent_devices.DeviceId} no existe.", }, 
            };
            return todosLosTests;
            /*yield return new object[] {}*/
        }





        [Fact]
        [Trait("LevelTesting", "Unit Testing")]
        public void PostReview_success_test()
        {

            // Arrange
            using var context = CreateContext();
            var mock = new Mock<ILogger<ReviewsController>>();
            ILogger<ReviewsController> logger = mock.Object;

            DateTime exact_time = DateTime.Now;

            //////////////////////////////////////////
            /////////////                /////////////
            /////////////  Post section  /////////////
            ////////////                 /////////////
            //////////////////////////////////////////
            CreateReviewDTO review_to_create = new CreateReviewDTO(
                "1",
                "Potencia brutal",
                "Italy",
                "Giorno",
                DateTime.Now,
                new List<ReviewItemDTO>()
                );

            ReviewItemDTO post_reviewItemDTO1 = new ReviewItemDTO(
                1,
                2025,
                5,
                "NVIDIA GeForce RTX 5090",
                "RTX 5090 Founders Edition",
                "Máximo rendimiento en gaming e IA"
                );

            ReviewItemDTO post_reviewItemDTO2 = new ReviewItemDTO(
                2,
                2025,
                4,
                "RTX 5080 Gaming Pro",
                "NVIDIA GeForce RTX 5080",
                "Buen rendimiento pero algo ruidosa"
            );

            review_to_create.ReviewItems.Add(post_reviewItemDTO1);
            review_to_create.ReviewItems.Add(post_reviewItemDTO2);


            //////////////////////////////////////////
            /////////////                /////////////
            /////////////   Get section  /////////////
            ////////////                 /////////////
            //////////////////////////////////////////
            var controller = new ReviewsController(context, logger);

            var reviewDetailsDTO_expected = new ReviewDetailsDTO(
                4,
                "Giorno",
                "Italy",
                "Potencia brutal",
                DateTime.Now, 
                new List<ReviewDeviceDTO>()
                );




            ReviewDeviceDTO device_expected1 = new ReviewDeviceDTO(
                1,
                "RTX 5090 Founders Edition",
                "NVIDIA GeForce RTX 5090",
                2025,
                5,
                "Máximo rendimiento en gaming e IA"
                );

            ReviewDeviceDTO device_expected2 = new ReviewDeviceDTO(
                2,
                "RTX 5080 Gaming Pro",
                "NVIDIA GeForce RTX 5080",
                2025,
                4,
                "Buen rendimiento pero algo ruidosa"
            );
            reviewDetailsDTO_expected.ReviewDetailsDevicesList.Add(device_expected1);
            reviewDetailsDTO_expected.ReviewDetailsDevicesList.Add(device_expected2);
            // Act
            var result = controller.CreateReview(review_to_create);

            //Assert
            var expected_reviewDetails = Assert.IsType<CreatedAtActionResult>(result.Result.Result).Value;

            Assert.Equal(reviewDetailsDTO_expected, expected_reviewDetails);

        }

        [Theory]
        [Trait("LevelTesting", "Unit Testing")]
        [MemberData(nameof(CaseTest_Error_PostReview))]
        public void CreateReview_Error_test(CreateReviewDTO? expected_Review, string expected_error)
        {
            // Arrange
            using var context = CreateContext();
            var mock = new Mock<ILogger<ReviewsController>>();
            ILogger<ReviewsController> logger = mock.Object;

            var controller = new ReviewsController(context, logger);

            // Act
            var result = controller.CreateReview(expected_Review);

            //Assert
            var objFinal = Assert.IsType<BadRequestObjectResult>(result.Result.Result).Value;

            //we look for the error
            var errors = ((ValidationProblemDetails)objFinal).Errors;

            Assert.Equal(1, errors.Count);

            var errorActual = errors.First().Value;

            Assert.Equal(expected_error, errorActual[0]);

        }
    }
}
