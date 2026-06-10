namespace AppForSEII2526.UIT.Shared.CU_Compras;

public class UCPurchaseDevices_UIT : UC_UIT
{
    private readonly ListDevicesForPurchasePageObject selectDevices;
    private readonly CreatePurchasePageObject createPurchase;
    private readonly DetailPurchasePageObject detailPurchase;

    // Dispositivos disponibles en los datos de prueba utilizados por los flujos funcionales.
    private const string deviceName1 = "Galaxy S24";
    private const string deviceName2 = "iPhone 15";
    private const string deviceName3 = "Pixel 8";

    // Colores usados en los filtros de la pantalla de selección.
    private const string color1 = "Azul";
    private const string color2 = "Negro";
    private const string color3 = "Blanco";

    // Datos del segundo dispositivo, utilizados cuando se comprueba el contenido final del detalle.
    private const string brand2 = "Apple";
    private const string model2 = "Apple iPhone 15";
    private const string price2 = "999,99 €";
    private const string description2 = "Smartphone Apple iPhone 15 disponible para compra";

    // Datos del tercer dispositivo, utilizado en el flujo básico de compra.
    private const string brand3 = "Google";
    private const string model3 = "Google Pixel 8";
    private const string price3 = "799,99 €";
    private const string description3 = "Smartphone Google Pixel 8 disponible para compra";

    // Datos de cliente introducidos en el formulario de compra.
    private const string customerName = "Maria";
    private const string customerSurname = "Torres";
    private const string deliveryAddress = "Albacete";

    public UCPurchaseDevices_UIT(ITestOutputHelper output) : base(output)
    {
        selectDevices = new ListDevicesForPurchasePageObject(_driver, _output);
        createPurchase = new CreatePurchasePageObject(_driver, _output);
        detailPurchase = new DetailPurchasePageObject(_driver, _output);
    }

    private void InitialStepsForPurchaseDevices_UIT()
    {
        // Se inicia sesión con un usuario cliente antes de acceder al caso de uso.
        Perform_login("maria@uclm.es", "Aa123456789@");

        Thread.Sleep(500);

        // Se accede directamente a la pantalla de selección de dispositivos para compra.
        _driver.Navigate().GoToUrl(_URI + "purchase/selectdevicesforpurchase");

        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));

        // Se espera a que el navegador haya terminado de cargar la página.
        wait.Until(d =>
        {
            var readyState = ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState");
            return readyState?.ToString() == "complete";
        });

        // La pantalla se considera cargada cuando aparece el buscador y, además,
        // la tabla de dispositivos, el mensaje de ausencia de dispositivos o el título de la página.
        wait.Until(d =>
            d.FindElements(By.Id("searchDevices")).Count > 0 &&
            (
                d.FindElements(By.Id("TableOfDevices")).Count > 0 ||
                d.FindElements(By.Id("noDevicesMessage")).Count > 0 ||
                d.PageSource.ToLower().Contains("comprar dispositivo")
            ));
    }

    [Theory]
    [Trait("LevelTesting", "Functional Testing")]
    [InlineData("CreditCard")]
    [InlineData("PayPal")]
    public void UC1_BasicFlow_AllPaymentMethods(string paymentMethod)
    {
        // Arrange
        InitialStepsForPurchaseDevices_UIT();

        // Act
        // Se añade un dispositivo al carrito y se avanza al formulario de compra.
        selectDevices.AddDevice(deviceName3);
        selectDevices.ContinuePurchase();

        var waitForm = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));
        waitForm.Until(d => d.FindElements(By.Id("purchaseName")).Count > 0);

        // Se rellenan los datos del cliente y se selecciona el método de pago indicado.
        createPurchase.FillCustomerData(customerName, customerSurname, deliveryAddress);
        createPurchase.SelectPaymentMethod(paymentMethod);

        Thread.Sleep(500);

        // Se guarda la compra y se espera a que se muestre la pantalla de detalle.
        createPurchase.SavePurchaseAndWaitForDetail();

        _output.WriteLine($"[{paymentMethod}] URL FINAL: " + _driver.Url);

        // Assert
        // La navegación final debe corresponder al detalle de la compra.
        Assert.Contains("detailpurchase", _driver.Url.ToLower());
        Assert.True(detailPurchase.IsLoaded());

        // Se comprueban los datos personales mostrados en el detalle.
        Assert.Contains(customerName, detailPurchase.GetCustomerName());
        Assert.Contains(customerSurname, detailPurchase.GetCustomerSurname());
        Assert.Contains(deliveryAddress, detailPurchase.GetDeliveryAddress());

        // Se espera que el detalle incluya el dispositivo comprado en el flujo básico.
        var expectedPurchaseItems = new List<string[]>
        {
            new string[]
            {
                brand3,
                model3,
                color3,
                price3,
                "1"
            }
        };

        Assert.True(
            detailPurchase.CheckListOfPurchasedDevices(expectedPurchaseItems),
            "Error: los dispositivos comprados no son los esperados");

        // Se comprueba que el detalle muestra los bloques de cantidad y precio total.
        Assert.Contains("Cantidad total", detailPurchase.GetTotalQuantity());
        Assert.Contains("Precio total", detailPurchase.GetTotalPrice());
    }

    [Fact]
    [Trait("LevelTesting", "Functional Testing")]
    public void UC2_1_NoDevicesAvailable()
    {
        // Arrange
        InitialStepsForPurchaseDevices_UIT();

        // Act
        // Se realiza una búsqueda con valores que no deberían devolver resultados.
        selectDevices.SearchByNameAndColor("Dispositivo inexistente", "Color inexistente");

        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        wait.Until(d =>
            d.FindElements(By.Id("noDevicesMessage")).Count > 0 ||
            d.PageSource.ToLower().Contains("no hay dispositivos"));

        // Assert
        // La pantalla debe informar de que no existen dispositivos que cumplan los filtros.
        Assert.Contains("no hay dispositivos", _driver.PageSource.ToLower());
    }

    [Fact]
    [Trait("LevelTesting", "Functional Testing")]
    public void UC3_1_FilterByName()
    {
        // Arrange
        InitialStepsForPurchaseDevices_UIT();

        // Act
        // Se filtra por nombre para localizar un dispositivo concreto.
        selectDevices.SearchByName(deviceName2);

        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));
        wait.Until(d => d.PageSource.Contains(deviceName2));

        // Assert
        // Debe aparecer el dispositivo buscado y no otro que no cumple el filtro.
        Assert.Contains(deviceName2, _driver.PageSource);
        Assert.DoesNotContain(deviceName1, _driver.PageSource);
    }

    [Fact]
    [Trait("LevelTesting", "Functional Testing")]
    public void UC3_2_FilterByColor()
    {
        // Arrange
        InitialStepsForPurchaseDevices_UIT();

        // Act
        // Se filtra por color para comprobar que el listado se actualiza.
        selectDevices.SearchByColor(color2);

        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));
        wait.Until(d => d.PageSource.Contains(deviceName2));

        // Assert
        // El dispositivo asociado al color buscado debe aparecer en los resultados.
        Assert.Contains(deviceName2, _driver.PageSource);
    }

    [Fact]
    [Trait("LevelTesting", "Functional Testing")]
    public void UC3_3_FilterByNameAndColor()
    {
        // Arrange
        InitialStepsForPurchaseDevices_UIT();

        // Act
        // Se aplican simultáneamente filtros de nombre y color.
        selectDevices.SearchByNameAndColor(deviceName3, color3);

        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));
        wait.Until(d => d.PageSource.Contains(deviceName3));

        // Assert
        // Debe mostrarse el dispositivo que cumple ambos criterios de búsqueda.
        Assert.Contains(deviceName3, _driver.PageSource);
    }

    [Fact]
    [Trait("LevelTesting", "Functional Testing")]
    public void UC4_1_ModifyPurchaseCart()
    {
        // Arrange
        InitialStepsForPurchaseDevices_UIT();

        // Act
        // Se añaden dos dispositivos al carrito.
        selectDevices.AddDevice(deviceName1);
        selectDevices.AddDevice(deviceName2);

        // Se aumenta la cantidad del primer dispositivo y después se elimina del carrito.
        selectDevices.IncreaseFirstDeviceQuantity();
        selectDevices.RemoveFirstDeviceFromCart();

        // Se avanza al formulario de compra con el carrito ya modificado.
        selectDevices.ContinuePurchase();

        var waitForm = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));
        waitForm.Until(d => d.FindElements(By.Id("purchaseName")).Count > 0);

        // Assert
        // El formulario debe mostrar los totales calculados a partir del carrito actual.
        Assert.Contains("Cantidad total", createPurchase.GetTotalQuantity());
        Assert.Contains("Total", createPurchase.GetTotalPrice());

        var selectedDevicesText = createPurchase.GetSelectedDevicesText();

        // El dispositivo eliminado no debe aparecer, mientras que el que permanece sí.
        Assert.DoesNotContain(deviceName1, selectedDevicesText);
        Assert.Contains(deviceName2, selectedDevicesText);
    }

    [Fact]
    [Trait("LevelTesting", "Functional Testing")]
    public void UC5_1_EmptyCart()
    {
        // Arrange
        InitialStepsForPurchaseDevices_UIT();

        // Act
        // Se añade y elimina un dispositivo para dejar el carrito vacío.
        selectDevices.AddDevice(deviceName1);
        selectDevices.RemoveFirstDeviceFromCart();

        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        wait.Until(d =>
            d.FindElement(By.Id("purchaseDevicesButton")).GetAttribute("disabled") != null);

        var purchaseButton = _driver.FindElement(By.Id("purchaseDevicesButton"));
        var isDisabled = purchaseButton.GetAttribute("disabled") != null;

        // Assert
        // Con el carrito vacío no debe permitirse continuar a la compra.
        Assert.True(isDisabled);
        Assert.True(selectDevices.IsEmptyCartMessageVisible());
    }

    [Theory]
    [Trait("LevelTesting", "Functional Testing")]
    [InlineData("", "Torres", "Albacete", "purchaseNameError")]
    [InlineData("Maria", "", "Albacete", "purchaseSurnameError")]
    [InlineData("Maria", "Torres", "", "purchaseDeliveryAddressError")]
    public void UC6_ValidationErrors(
        string name,
        string surname,
        string address,
        string expectedErrorId)
    {
        // Arrange
        InitialStepsForPurchaseDevices_UIT();

        // Act
        // Se añade un dispositivo válido para llegar al formulario de compra.
        selectDevices.AddDevice(deviceName1);
        selectDevices.ContinuePurchase();

        var waitForm = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        waitForm.Until(d => d.FindElements(By.Id("purchaseName")).Count > 0);

        // Se rellenan los datos recibidos por parámetro para provocar una validación concreta.
        createPurchase.FillCustomerData(name, surname, address);
        createPurchase.SelectPaymentMethod("CreditCard");
        createPurchase.SavePurchase();

        var waitError = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        waitError.Until(d => d.FindElements(By.Id(expectedErrorId)).Any(e => e.Displayed));

        // Assert
        // Debe mostrarse el mensaje de validación asociado al campo correspondiente.
        Assert.True(_driver.FindElement(By.Id(expectedErrorId)).Displayed);
    }

    [Fact]
    [Trait("LevelTesting", "Functional Testing")]
    public void UC7_1_ModifySelectedDevicesAndKeepFormData()
    {
        // Arrange
        InitialStepsForPurchaseDevices_UIT();

        // Act
        // Se selecciona un dispositivo y se accede al formulario de compra.
        selectDevices.AddDevice(deviceName1);
        selectDevices.ContinuePurchase();

        var waitForm = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        waitForm.Until(d => d.FindElements(By.Id("purchaseName")).Count > 0);

        // Se rellenan datos del cliente antes de volver a modificar el carrito.
        createPurchase.FillCustomerData(customerName, customerSurname, deliveryAddress);
        createPurchase.SelectPaymentMethod("CreditCard");

        // Se vuelve al listado para modificar los dispositivos seleccionados.
        createPurchase.ModifyCartAndWaitForSelection();

        // Se añade otro dispositivo y se regresa al formulario.
        selectDevices.AddDevice(deviceName3);
        selectDevices.ContinuePurchase();

        waitForm.Until(d => d.FindElements(By.Id("purchaseName")).Count > 0);

        // Assert
        // Los datos ya introducidos deben conservarse al volver al formulario.
        Assert.Equal(customerName, _driver.FindElement(By.Id("purchaseName")).GetAttribute("value"));
        Assert.Equal(customerSurname, _driver.FindElement(By.Id("purchaseSurname")).GetAttribute("value"));
        Assert.Equal(deliveryAddress, _driver.FindElement(By.Id("purchaseDeliveryAddress")).GetAttribute("value"));
    }

    [Theory]
    [InlineData("CreditCard")]
    [InlineData("PayPal")]
    [Trait("LevelTesting", "Functional Testing")]
    [Trait("UseCase", "Comprar dispositivo")]
    public void UC1_8_ModifyCartAndPurchase(string paymentMethod)
    {
        // Arrange
        InitialStepsForPurchaseDevices_UIT();

        // Act
        // Se selecciona un primer dispositivo para iniciar la compra.
        selectDevices.AddDevice(deviceName3);

        // Se avanza al formulario con el carrito inicial.
        selectDevices.ContinuePurchase();

        var waitForm = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));
        waitForm.Until(d => d.FindElements(By.Id("purchaseName")).Count > 0);

        // Desde el formulario se vuelve a la selección para modificar el carrito.
        createPurchase.ModifyCartAndWaitForSelection();

        var waitSelection = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));
        waitSelection.Until(d => d.FindElements(By.Id("TableOfDevices")).Count > 0);

        // Se filtra por color para localizar el segundo dispositivo que se va a comprar.
        selectDevices.SearchByColor(color2);

        var waitDevice = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));
        waitDevice.Until(d => d.PageSource.Contains(deviceName2));

        // Se comprueba que el dispositivo que se quiere añadir está disponible en la página.
        Assert.Contains(deviceName2, _driver.PageSource);

        // Se añade el segundo dispositivo al carrito.
        selectDevices.AddDevice(deviceName2);

        // Se elimina el primer dispositivo para que la compra final solo incluya el segundo.
        selectDevices.RemoveFirstDeviceFromCart();

        // Se vuelve al formulario con el carrito definitivo.
        selectDevices.ContinuePurchase();

        waitForm.Until(d => d.FindElements(By.Id("purchaseName")).Count > 0);

        // Se rellenan los datos necesarios para confirmar la compra.
        createPurchase.FillCustomerData(
            customerName,
            customerSurname,
            deliveryAddress);

        createPurchase.SelectPaymentMethod(paymentMethod);

        // Se guarda la compra y se espera a la pantalla de detalle.
        createPurchase.SavePurchaseAndWaitForDetail();

        _output.WriteLine($"[{paymentMethod}] URL FINAL: " + _driver.Url);

        // Assert
        // Tras guardar correctamente, la navegación debe terminar en el detalle de compra.
        Assert.Contains("detailpurchase", _driver.Url.ToLower());
        Assert.True(detailPurchase.IsLoaded());

        // Se comprueban los datos personales mostrados en el detalle.
        Assert.Contains(customerName, detailPurchase.GetCustomerName());
        Assert.Contains(customerSurname, detailPurchase.GetCustomerSurname());
        Assert.Contains(deliveryAddress, detailPurchase.GetDeliveryAddress());

        // Solo debe aparecer el dispositivo que quedó en el carrito tras la modificación.
        var expectedPurchaseItems = new List<string[]>
        {
            new string[]
            {
                brand2,
                model2,
                color2,
                price2,
                "1",
                price2
            }
        };

        Assert.True(
            detailPurchase.CheckListOfPurchasedDevices(expectedPurchaseItems),
            "Error: los dispositivos comprados no son los esperados");

        // Se comprueba que el resumen final muestra cantidad y precio total.
        Assert.Contains("Cantidad total", detailPurchase.GetTotalQuantity());
        Assert.Contains("Precio total", detailPurchase.GetTotalPrice());
    }
}