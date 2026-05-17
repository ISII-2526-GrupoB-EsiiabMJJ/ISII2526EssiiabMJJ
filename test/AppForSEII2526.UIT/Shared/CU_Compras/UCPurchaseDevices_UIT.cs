namespace AppForSEII2526.UIT.Shared.CU_Compras;

public class UCPurchaseDevices_UIT : UC_UIT
{
    private readonly ListDevicesForPurchasePageObject selectDevices;
    private readonly CreatePurchasePageObject createPurchase;
    private readonly DetailPurchasePageObject detailPurchase;

    private const string deviceName1 = "Galaxy S24";
    private const string deviceName2 = "iPhone 15";
    private const string deviceName3 = "Pixel 8";

    private const string color1 = "Azul";
    private const string color2 = "Negro";
    private const string color3 = "Blanco";

    private const string brand3 = "Google";
    private const string model3 = "Google Pixel 8";
    private const string price3 = "799,99 €";
    private const string description3 = "Smartphone Google Pixel 8 disponible para compra";

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
        Perform_login("maria@uclm.es", "Aa123456789@");

        Thread.Sleep(500);

        _driver.Navigate().GoToUrl(_URI + "purchase/selectdevicesforpurchase");

        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));

        wait.Until(d =>
        {
            var readyState = ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState");
            return readyState?.ToString() == "complete";
        });

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
        InitialStepsForPurchaseDevices_UIT();

        selectDevices.AddDevice(deviceName3);
        selectDevices.ContinuePurchase();

        var waitForm = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));
        waitForm.Until(d => d.FindElements(By.Id("purchaseName")).Count > 0);

        createPurchase.FillCustomerData(customerName, customerSurname, deliveryAddress);
        createPurchase.SelectPaymentMethod(paymentMethod);
        Thread.Sleep(500);
        createPurchase.SavePurchaseAndWaitForDetail();

        _output.WriteLine($"[{paymentMethod}] URL FINAL: " + _driver.Url);

        Assert.Contains("detailpurchase", _driver.Url.ToLower());
        Assert.True(detailPurchase.IsLoaded());
        Assert.Contains(customerName, detailPurchase.GetCustomerName());
        Assert.Contains(customerSurname, detailPurchase.GetCustomerSurname());
        Assert.Contains(deliveryAddress, detailPurchase.GetDeliveryAddress());
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

        Assert.True(detailPurchase.CheckListOfPurchasedDevices(expectedPurchaseItems),
            "Error: los dispositivos comprados no son los esperados");
        Assert.Contains("Cantidad total", detailPurchase.GetTotalQuantity());
        Assert.Contains("Precio total", detailPurchase.GetTotalPrice());
    }

    [Fact]
    [Trait("LevelTesting", "Functional Testing")]
    public void UC2_1_NoDevicesAvailable()
    {
        InitialStepsForPurchaseDevices_UIT();

        selectDevices.SearchByNameAndColor("Dispositivo inexistente", "Color inexistente");

        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        wait.Until(d =>
            d.FindElements(By.Id("noDevicesMessage")).Count > 0 ||
            d.PageSource.ToLower().Contains("no hay dispositivos"));

        Assert.Contains("no hay dispositivos", _driver.PageSource.ToLower());
    }

    [Fact]
    [Trait("LevelTesting", "Functional Testing")]
    public void UC3_1_FilterByName()
    {
        InitialStepsForPurchaseDevices_UIT();

        selectDevices.SearchByName(deviceName2);

        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));
        wait.Until(d => d.PageSource.Contains(deviceName2));

        Assert.Contains(deviceName2, _driver.PageSource);
        Assert.DoesNotContain(deviceName1, _driver.PageSource);
    }

    [Fact]
    [Trait("LevelTesting", "Functional Testing")]
    public void UC3_2_FilterByColor()
    {
        InitialStepsForPurchaseDevices_UIT();

        selectDevices.SearchByColor(color2);

        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));
        wait.Until(d => d.PageSource.Contains(deviceName2));

        Assert.Contains(deviceName2, _driver.PageSource);
    }

    [Fact]
    [Trait("LevelTesting", "Functional Testing")]
    public void UC3_3_FilterByNameAndColor()
    {
        InitialStepsForPurchaseDevices_UIT();

        selectDevices.SearchByNameAndColor(deviceName3, color3);

        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));
        wait.Until(d => d.PageSource.Contains(deviceName3));

        Assert.Contains(deviceName3, _driver.PageSource);
    }

    [Fact]
    [Trait("LevelTesting", "Functional Testing")]
    public void UC4_1_ModifyPurchaseCart()
    {
        InitialStepsForPurchaseDevices_UIT();

        selectDevices.AddDevice(deviceName1);
        selectDevices.AddDevice(deviceName2);

        selectDevices.IncreaseFirstDeviceQuantity();
        selectDevices.RemoveFirstDeviceFromCart();

        selectDevices.ContinuePurchase();

        var waitForm = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));
        waitForm.Until(d => d.FindElements(By.Id("purchaseName")).Count > 0);

        Assert.Contains("Cantidad total", createPurchase.GetTotalQuantity());
        Assert.Contains("Total", createPurchase.GetTotalPrice());

        var selectedDevicesText = createPurchase.GetSelectedDevicesText();

        Assert.DoesNotContain(deviceName1, selectedDevicesText);
        Assert.Contains(deviceName2, selectedDevicesText);
    }

    [Fact]
    [Trait("LevelTesting", "Functional Testing")]
    public void UC5_1_EmptyCart()
    {
        InitialStepsForPurchaseDevices_UIT();

        selectDevices.AddDevice(deviceName1);
        selectDevices.RemoveFirstDeviceFromCart();

        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        wait.Until(d =>
            d.FindElement(By.Id("purchaseDevicesButton")).GetAttribute("disabled") != null);

        var purchaseButton = _driver.FindElement(By.Id("purchaseDevicesButton"));
        var isDisabled = purchaseButton.GetAttribute("disabled") != null;

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
        InitialStepsForPurchaseDevices_UIT();

        selectDevices.AddDevice(deviceName1);
        selectDevices.ContinuePurchase();

        var waitForm = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        waitForm.Until(d => d.FindElements(By.Id("purchaseName")).Count > 0);

        createPurchase.FillCustomerData(name, surname, address);
        createPurchase.SelectPaymentMethod("CreditCard");
        createPurchase.SavePurchase();

        var waitError = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        waitError.Until(d => d.FindElements(By.Id(expectedErrorId)).Any(e => e.Displayed));

        Assert.True(_driver.FindElement(By.Id(expectedErrorId)).Displayed);
    }

    [Fact]
    [Trait("LevelTesting", "Functional Testing")]
    public void UC7_1_ModifySelectedDevicesAndKeepFormData()
    {
        InitialStepsForPurchaseDevices_UIT();

        selectDevices.AddDevice(deviceName1);
        selectDevices.ContinuePurchase();

        var waitForm = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        waitForm.Until(d => d.FindElements(By.Id("purchaseName")).Count > 0);

        createPurchase.FillCustomerData(customerName, customerSurname, deliveryAddress);
        createPurchase.SelectPaymentMethod("CreditCard");

        createPurchase.ModifyCartAndWaitForSelection();

        selectDevices.AddDevice(deviceName3);
        selectDevices.ContinuePurchase();

        waitForm.Until(d => d.FindElements(By.Id("purchaseName")).Count > 0);

        Assert.Equal(customerName, _driver.FindElement(By.Id("purchaseName")).GetAttribute("value"));
        Assert.Equal(customerSurname, _driver.FindElement(By.Id("purchaseSurname")).GetAttribute("value"));
        Assert.Equal(deliveryAddress, _driver.FindElement(By.Id("purchaseDeliveryAddress")).GetAttribute("value"));
    }
}