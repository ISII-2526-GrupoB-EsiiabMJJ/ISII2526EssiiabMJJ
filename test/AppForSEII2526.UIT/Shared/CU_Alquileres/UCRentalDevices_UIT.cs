using AppForSEII2526.UIT.Shared.CU_Alquileres;

public class UCRentalDevices_UIT : UC_UIT
{
    private ListDevicesForRental_PO listDevices;
    private CreateRental_PO createRental;
    private DetailRental_PO detailRental;

    private const string deviceModel1 = "NVIDIA GeForce RTX 5090";
    private const string deviceName1 = "RTX 5090 Founders Edition";
    private const string devicePrice1 = "39.99";
    private const string deviceDescription1 = "Random comment for test";

    
    private List<string> chosedDevices = new List<string> {
        "RTX 5090 Founders Edition",
        "RTX 5080 Gaming Pro",
        "RTX 4080 Ti Dual Fan",
        "RTX 4090 Dual Ultimate"
    };

    public UCRentalDevices_UIT(ITestOutputHelper output) : base(output)
    {
        listDevices = new ListDevicesForRental_PO(_driver, _output);
        createRental = new CreateRental_PO(_driver, _output);
        detailRental = new DetailRental_PO(_driver, _output);
    }


    
    private void InitialStepsForRentalDevices_UIT()
    {
        Perform_login("jaime@uclm.es", "Aa123456789@");
        Thread.Sleep(500);
        _driver.Navigate().GoToUrl(_URI + "rent/listdevicesforrenting");

   
        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15)); 

        
        wait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));

       
        wait.Until(d =>
            d.FindElements(By.Id("searchDevices")).Count > 0 ||
            d.FindElements(By.Id("devicesTable")).Count > 0 ||
            d.FindElements(By.CssSelector("button[id^='deviceToAdd_']")).Count > 0 ||
            d.PageSource.ToLower().Contains("device") ||  
            d.PageSource.ToLower().Contains("no devices")  
        );

       
        Thread.Sleep(1000); 
       
    }

    [Theory]
    [Trait("LevelTesting", "Functional Testing")]
    [InlineData("CreditCard")]
    [InlineData("PayPal")]
    [InlineData("Cash")]
    public void UC1_BasicFlow_AllPaymentMethods(string paymentMethod)
    {
        InitialStepsForRentalDevices_UIT();

        var from = DateTime.Today.AddDays(0);
        var to = DateTime.Today.AddDays(1);

        listDevices.FilterDevices("", "", from, to);

        // Verificar que el dispositivo está disponible antes de continuar
        var wait0 = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        wait0.Until(d =>
            d.FindElements(By.CssSelector($"button[id^='deviceToAdd_']")).Count > 0
        );

        listDevices.ClickAddDevice(deviceModel1);
        listDevices.clickButton(By.Id("Rent"));

        var waitForm = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        waitForm.Until(d => d.FindElements(By.Id("NameSurname")).Count > 0);

        createRental.FillInRentalInfo("Jaime Cátedra", "Calle Universidad 1", paymentMethod);
        createRental.PressRentYourDevices();

        try { createRental.PressOkModalDialog(); }
        catch { _output.WriteLine($"[{paymentMethod}] No modal de confirmación"); }

        var waitUrl = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        waitUrl.Until(d => d.Url.ToLower().Contains("detailrental"));

        _output.WriteLine($"[{paymentMethod}] URL FINAL: " + _driver.Url);
        Assert.Contains("detailrental", _driver.Url.ToLower());
    }

    // UC2_1 
    [Fact]
    [Trait("LevelTesting", "Functional Testing")]
    public void UC2_1_NoDevicesAvailable()
    {
        InitialStepsForRentalDevices_UIT();

        listDevices.FilterDevices("", "9999", DateTime.Today.AddYears(10), DateTime.Today.AddYears(10).AddDays(1));

        Assert.Contains("no devices available", _driver.PageSource.ToLower());
    }
   
    [Fact]
    [Trait("LevelTesting", "Functional Testing")]
    public void UC3_1_FilterByModel()
    {
        // Arrange
        InitialStepsForRentalDevices_UIT();

        var from = DateTime.Today.AddDays(1);
        var to = DateTime.Today.AddDays(2);

        var expectedDevices = new List<string[]>
    {
        new string[] { "RTX 4080 Ti Dual Fan", "NVIDIA GeForce RTX 4080 Ti", "Gigabyte", "2024", "Blanco", "39,99" }
    };

        // Act
        listDevices.FilterDevices("NVIDIA GeForce RTX 4080 Ti", "", from, to);

        // Assert
        Assert.True(listDevices.CheckListOfDevices(expectedDevices));
    }
    [Fact]
    [Trait("LevelTesting", "Functional Testing")]
    public void UC3_2_FilterByPrice()
    {
        // Arrange
        InitialStepsForRentalDevices_UIT();

        var from = DateTime.Today.AddDays(1);
        var to = DateTime.Today.AddDays(2);

        var expectedDevices = new List<string[]>
    {
        new string[] { "RTX 4080 Ti Dual Fan", "NVIDIA GeForce RTX 4080 Ti", "Gigabyte", "2024", "Blanco", "39,99" }
    };

        // Act
        listDevices.FilterDevices("", "39.99", from, to);

        // Assert
        Assert.True(listDevices.CheckListOfDevices(expectedDevices));
    }

    [Fact]
    [Trait("LevelTesting", "Functional Testing")]
    public void UC3_3_FilterByModelAndPrice()
    {
        // Arrange
        InitialStepsForRentalDevices_UIT();

        var from = DateTime.Today.AddDays(1);
        var to = DateTime.Today.AddDays(2);

        var expectedDevices = new List<string[]>
    {
        new string[]
        {
            "RTX 4080 Ti Dual Fan",
            "NVIDIA GeForce RTX 4080 Ti",
            "Gigabyte",
            "2024",
            "Blanco",
            "39,99"
        }
    };

       
        listDevices.FilterDevices(
            "NVIDIA GeForce RTX 4080 Ti",  
            "39.99",                      
            from,
            to
        );

        // Assert
        Assert.True(listDevices.CheckListOfDevices(expectedDevices));
    }

    // UC4_1 - Modificar carrito

    [Fact]
    [Trait("LevelTesting", "Functional Testing")]
    public void UC4_1_ModifyRentalCart()
    {
        InitialStepsForRentalDevices_UIT();

        var from = DateTime.Today.AddDays(1);
        var to = DateTime.Today.AddDays(2);

        // 1. Filtrar
        listDevices.FilterDevices("", "", from, to);

        // 2. Añadir 2 dispositivos
        listDevices.ClickAddDevice("NVIDIA GeForce RTX 5090");
        listDevices.ClickAddDevice("NVIDIA GeForce RTX 5080");

        // 3. Ir a carrito
        listDevices.clickButton(By.Id("Rent"));

        //createRental.PressRentYourDevices();

        // 4. Volver a modificar
        createRental.PressModifyDevices();

        listDevices.WaitForDevicesPage();

        // 5. Quitar uno
        listDevices.ClickRemoveDevice("NVIDIA GeForce RTX 5080");

        // 6. Guardar cambios
        listDevices.clickButton(By.Id("Rent"));


        
        var stillExists = _driver
            .FindElements(By.Id("RentalItem_NVIDIA GeForce RTX 5080"))
            .Count > 0;

        Assert.True(!stillExists);
    }
    [Fact]
    [Trait("LevelTesting", "Functional Testing")]
    public void UC5_1_EmptyCart()
    {
        InitialStepsForRentalDevices_UIT();

        var from = DateTime.Today.AddDays(1);
        var to = DateTime.Today.AddDays(2);
        Thread.Sleep(300);
        // 1. Filtrar
        listDevices.FilterDevices("", "", from, to);
        Thread.Sleep(300);
        // 2. Seleccionar RTX 5080
        listDevices.ClickAddDevice("NVIDIA GeForce RTX 5080");
        Thread.Sleep(300);
        // 3. Deseleccionar RTX 5080
        listDevices.ClickRemoveDevice("NVIDIA GeForce RTX 5080");

        // 4. Esperar que Blazor actualice estado
        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        wait.Until(d =>
            d.FindElement(By.Id("Rent")).GetAttribute("disabled") != null
        );

        // 5. ASSERT REAL
        var rentButton = _driver.FindElement(By.Id("Rent"));
        bool isDisabled = rentButton.GetAttribute("disabled") != null;

        Assert.True(isDisabled);
    }

   
    [Theory]
    [Trait("LevelTesting", "Functional Testing")]
    [InlineData("", "Cátedra", "Calle Ejemplo 1", "CreditCard", "CustomerNameSurname")]
    [InlineData("Jaime", "", "Calle Ejemplo 1", "CreditCard", "CustomerNameSurname")]
    [InlineData("Jaime", "Cátedra", "", "CreditCard", "DeliveryAddress")]
   
    public void UC6_ValidationErrors(
    string name, string surname, string address,
    string payment, string expectedFieldName)
    {
        InitialStepsForRentalDevices_UIT();

        listDevices.FilterDevices("", "",
            DateTime.Today.AddDays(1), DateTime.Today.AddDays(2));

        listDevices.ClickAddDevice("NVIDIA GeForce RTX 5090");
        listDevices.clickButton(By.Id("Rent"));

        var waitForm = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        waitForm.Until(d => d.FindElements(By.Id("NameSurname")).Count > 0);

        string fullName = (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(surname))
            ? ""
            : $"{name} {surname}".Trim();

        createRental.FillInRentalInfo(fullName, address, payment);
        createRental.PressRentYourDevices();

        
        var waitError = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        waitError.Until(d =>
            d.FindElements(By.CssSelector(".validation-errors, .validation-message"))
             .Any(e => e.Displayed && !string.IsNullOrWhiteSpace(e.Text))
        );

       
        string allErrorText = string.Join(" ",
            _driver.FindElements(By.CssSelector(".validation-errors, .validation-message"))
                   .Where(e => e.Displayed)
                   .Select(e => e.Text)
        );

        _output.WriteLine($"Validation text: {allErrorText}");

        
        Assert.Contains(expectedFieldName.ToLower(), allErrorText.ToLower());
    }

    // UC7_1 - Modificar dispositivos seleccionados (corregido)
    [Fact]
    [Trait("LevelTesting", "Functional Testing")]
    public void UC7_1_ModifySelectedDevices()
    {
        InitialStepsForRentalDevices_UIT();

        //  1: Filtrar dispositivos
        listDevices.FilterDevices("", "", DateTime.Today.AddDays(1), DateTime.Today.AddDays(2));

        //  2: Seleccionar 2 dispositivos
        listDevices.ClickAddDevice("NVIDIA GeForce RTX 5090");
        listDevices.ClickAddDevice("NVIDIA GeForce RTX 5080");

        //  3: Ir al formulario
        listDevices.clickButton(By.Id("Rent"));

        //  4: Rellenar datos del cliente
        createRental.FillInRentalInfo("Jaime Cátedra", "Calle Universidad 1", "CreditCard");

        //  5: Volver a la lista de dispositivos
        createRental.PressModifyDevices();

        
        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));
        wait.Until(d =>
            d.FindElements(By.CssSelector("button[id^='deviceToAdd_']")).Count > 0 ||
            d.FindElements(By.Id("devicesTable")).Count > 0
        );

        //  6: Quitar RTX 5080 del carrito
        listDevices.ClickRemoveDevice("NVIDIA GeForce RTX 5080");

        //  7: Volver al formulario
        listDevices.clickButton(By.Id("Rent"));

        //  8: Esperar que cargue el formulario de nuevo
        var wait2 = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        wait2.Until(d => d.FindElements(By.Id("NameSurname")).Count > 0);

        // ASSERT
        bool persisted = createRental.CheckCreateRentalPersistance(
            "Jaime Cátedra",
            "Calle Universidad 1",
            "CreditCard"
        );

        Assert.True(persisted);
    }
  
}