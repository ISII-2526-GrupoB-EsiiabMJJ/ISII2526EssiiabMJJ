using AppForSEII2526.API.Controllers;
using AppForSEII2526.API.DTOs;
using AppForSEII2526.API.DTOs.ReviewDTOs;
using Xunit.Abstractions;

namespace AppForSEII2526.UT.ReviewControllers_test
{
    public class DeviceController_test
    {
        private readonly DbConnection _connection;
        private readonly DbContextOptions<ApplicationDbContext> _contextOptions;
        private readonly ITestOutputHelper _output;

        ApplicationDbContext CreateContext() => new(_contextOptions);
        void Dispose() => _connection.Dispose();

        public DeviceController_test(ITestOutputHelper output)
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

        public static IEnumerable<object[]> CasosTest_GetDevices()
        {
            var deviceDTOs = new List<DeviceDetailsDTO>
            {
                new DeviceDetailsDTO(3,"RTX 4080 Ti Dual Fan", "Gigabyte", "Blanco", 2024,"NVIDIA GeForce RTX 4080 Ti"),
                new DeviceDetailsDTO(5,"RTX 4090 Dual Ultimate", "MSI", "Negro", 2024, "NVIDIA GeForce RTX 4090"),
                new DeviceDetailsDTO(2,"RTX 5080 Gaming Pro", "MSI", "Plata",2025, "NVIDIA GeForce RTX 5080"),
                new DeviceDetailsDTO(1,"RTX 5090 Founders Edition", "NVIDIA", "Negro", 2025,"NVIDIA GeForce RTX 5090"),
                new DeviceDetailsDTO(4,"RTX 5090 OC Edition", "Gigabyte" ,"Rojo", 2025,"NVIDIA GeForce RTX 5090"),
            };


            var dto0 = new List<DeviceDetailsDTO>() { //Insert (null, null)
                    deviceDTOs[0], deviceDTOs[1], deviceDTOs[2], deviceDTOs[3], deviceDTOs[4]}
                .OrderBy(d => d.Name)
                .ToList();

            var dto1 = new List<DeviceDetailsDTO>() { //Insert ("MSI", null)
                    deviceDTOs[1],deviceDTOs[2],}
                .OrderBy(d => d.Name)
                .ToList();

            var dto2 = new List<DeviceDetailsDTO>() { //Insert (null, 2024)
                    deviceDTOs[0], deviceDTOs[1]}
                .OrderBy(d => d.Name)
                .ToList();

            var dto3 = new List<DeviceDetailsDTO>() { //Insert ("MSI", 2025)
                    deviceDTOs[2]}
                .OrderBy(d => d.Name)
                .ToList();


            var todoslosTests = new List<object[]>
            {
                new object[] { null, null, dto0, },
                new object[] { "MSI", null, dto1, },
                new object[] { null, 2024, dto2, },
                new object[] { "MSI", 2025, dto3, },
            };

            return todoslosTests;

        }



        [Theory]
        [MemberData(nameof(CasosTest_GetDevices))]
        [Trait("LevelTesting", "Unit Testing")]
        public void GetDevices_test(string? brand, int? year,
            IList<DeviceDetailsDTO> dispositivosEsperadas)
        {
            // Arrange
            using var context = CreateContext();
            var controller = new DevicesController(context, null);

            // Act
            var result = controller.DeviceInfo(brand, year);

            //Assert
            //we check that the response type is OK and obtain the list of movies
            var dispositivosRecibidos = Assert.IsType<OkObjectResult>(result.Result.Result).Value;



            //we check that the expected and actual are the same
            Assert.Equal(dispositivosEsperadas, dispositivosRecibidos);

        }

    }
}