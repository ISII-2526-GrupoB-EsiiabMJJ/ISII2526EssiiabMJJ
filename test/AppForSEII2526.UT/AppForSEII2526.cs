namespace AppForSEII2526.UT {
    public class AppForSEII25264SqliteUT
    {
        protected readonly DbConnection _connection;
        protected readonly ApplicationDbContext _context;
        protected readonly DbContextOptions<ApplicationDbContext> _contextOptions;

        protected ApplicationDbContext CreateContext() => new(_contextOptions);

        void Dispose() => _connection.Dispose();
        public AppForSEII25264SqliteUT() {
            // Create and open a connection. This creates the SQLite in-memory database, which will persist until the connection is closed
            // at the end of the test (see Dispose below).
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            _contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(_connection)
                .Options;

            // Crear contexto y esquema
            _context = new ApplicationDbContext(_contextOptions);
            _context.Database.EnsureCreated();
        }

        public static IEnumerable<object[]> TestCasesFor_GetDeviceForRental_OK()
        {
            var deviceDTOs = new List<DeviceRentalDTO>()
            {
                new DeviceRentalDTO(1, 2025, 59.99, "RTX 5090 Founders Edition", "NVIDIA", "Negro", "rtx"),
                new DeviceRentalDTO(2, 2025, 49.99, "RTX 5080 Gaming Pro", "MSI", "Plata", "rtx"),
                new DeviceRentalDTO(3, 2025, 39.99, "RTX 4080 Ti Dual Fan", "Gigabyte", "Blanco", "rtx"),
                new DeviceRentalDTO(4, 2025, 64.99, "RTX 5090 OC Edition", "ASUS", "Negro/Rojo", "rtx")
            };

            var allTests = new List<object[]>
            {
                new object[] { null, null, deviceDTOs.OrderBy(d => d.Name).ToList() },
                new object[] { "Gaming", null, deviceDTOs.Where(d => d.Name.Contains("Gaming")).ToList() },
                new object[] { null, "Gigabyte", deviceDTOs.Where(d => d.Model == "Gigabyte").ToList() },
                new object[] { null, null, deviceDTOs.OrderBy(d => d.Name).ToList() }
            };

            return allTests;
        }
    }
}
