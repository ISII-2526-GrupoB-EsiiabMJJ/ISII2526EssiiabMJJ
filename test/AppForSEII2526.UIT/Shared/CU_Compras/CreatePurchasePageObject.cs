using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.Shared.CU_Compras;

public class CreatePurchasePageObject : PageObject
{
    // Identificadores de los campos principales del formulario de compra.
    private readonly By _purchaseName = By.Id("purchaseName");
    private readonly By _purchaseSurname = By.Id("purchaseSurname");
    private readonly By _purchaseDeliveryAddress = By.Id("purchaseDeliveryAddress");
    private readonly By _paymentMethodSelected = By.Id("paymentMethodSelected");

    // Botones y zonas de información de la pantalla de creación de compra.
    private readonly By _modifyPurchaseCart = By.Id("modifyPurchaseCart");
    private readonly By _savePurchase = By.Id("savePurchase");
    private readonly By _createPurchaseTotalQuantity = By.Id("createPurchaseTotalQuantity");
    private readonly By _createPurchaseTotalPrice = By.Id("createPurchaseTotalPrice");
    private readonly By _purchaseSelectedDevices = By.Id("purchaseSelectedDevices");

    public CreatePurchasePageObject(IWebDriver driver, ITestOutputHelper output)
        : base(driver, output)
    {
    }

    public void FillCustomerData(string name, string surname, string deliveryAddress)
    {
        // Antes de escribir en el formulario se espera a que todos los campos estén disponibles.
        WaitForCreatePurchasePage();

        // Se limpian los campos por si la página ya conservaba valores anteriores.
        _driver.FindElement(_purchaseName).Clear();
        _driver.FindElement(_purchaseName).SendKeys(name);

        _driver.FindElement(_purchaseSurname).Clear();
        _driver.FindElement(_purchaseSurname).SendKeys(surname);

        _driver.FindElement(_purchaseDeliveryAddress).Clear();
        _driver.FindElement(_purchaseDeliveryAddress).SendKeys(deliveryAddress);
    }

    public void SelectPaymentMethod(string paymentMethodValue)
    {
        // El método de pago se selecciona por el value del desplegable.
        WaitForBeingVisible(_paymentMethodSelected);

        var select = new SelectElement(_driver.FindElement(_paymentMethodSelected));
        select.SelectByValue(paymentMethodValue);
    }

    public void SavePurchase()
    {
        // Guarda la compra sin esperar navegación. Es útil cuando el test quiere comprobar validaciones.
        WaitForBeingClickable(_savePurchase);
        _driver.FindElement(_savePurchase).Click();
    }

    public string GetTotalQuantity()
    {
        // Devuelve el texto de cantidad total mostrado antes de confirmar la compra.
        WaitForBeingVisible(_createPurchaseTotalQuantity);
        return _driver.FindElement(_createPurchaseTotalQuantity).Text;
    }

    public string GetTotalPrice()
    {
        // Devuelve el texto de precio total mostrado antes de confirmar la compra.
        WaitForBeingVisible(_createPurchaseTotalPrice);
        return _driver.FindElement(_createPurchaseTotalPrice).Text;
    }

    public void WaitForCreatePurchasePage()
    {
        // La página se considera cargada cuando están visibles los campos obligatorios
        // y el selector del método de pago.
        WaitForBeingVisible(_purchaseName);
        WaitForBeingVisible(_purchaseSurname);
        WaitForBeingVisible(_purchaseDeliveryAddress);
        WaitForBeingVisible(_paymentMethodSelected);
    }

    public void SavePurchaseAndWaitForDetail()
    {
        // Se confirma la compra y se espera a que el navegador llegue a la pantalla de detalle.
        WaitForBeingClickable(_savePurchase);
        _driver.FindElement(_savePurchase).Click();

        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));
        wait.Until(d => d.Url.ToLower().Contains("detailpurchase"));
    }

    public void ModifyCartAndWaitForSelection()
    {
        // Desde el formulario se vuelve al listado para cambiar los dispositivos seleccionados.
        WaitForBeingClickable(_modifyPurchaseCart);
        _driver.FindElement(_modifyPurchaseCart).Click();

        // Se espera a que vuelva a estar visible la tabla de dispositivos disponibles.
        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));
        wait.Until(d => d.FindElements(By.Id("TableOfDevices")).Any());
    }

    public string GetSelectedDevicesText()
    {
        // Devuelve el resumen de dispositivos seleccionados mostrado en el formulario.
        WaitForBeingVisible(_purchaseSelectedDevices);
        return _driver.FindElement(_purchaseSelectedDevices).Text;
    }
}