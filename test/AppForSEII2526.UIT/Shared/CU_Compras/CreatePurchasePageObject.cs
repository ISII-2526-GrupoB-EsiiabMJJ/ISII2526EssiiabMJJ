using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.Shared.CU_Compras;

public class CreatePurchasePageObject : PageObject
{
    private readonly By _purchaseName = By.Id("purchaseName");
    private readonly By _purchaseSurname = By.Id("purchaseSurname");
    private readonly By _purchaseDeliveryAddress = By.Id("purchaseDeliveryAddress");
    private readonly By _paymentMethodSelected = By.Id("paymentMethodSelected");
    private readonly By _modifyPurchaseCart = By.Id("modifyPurchaseCart");
    private readonly By _savePurchase = By.Id("savePurchase");
    private readonly By _createPurchaseTotalQuantity = By.Id("createPurchaseTotalQuantity");
    private readonly By _createPurchaseTotalPrice = By.Id("createPurchaseTotalPrice");
    public CreatePurchasePageObject(IWebDriver driver, ITestOutputHelper output)
        : base(driver, output)
    {
    }

    public void FillCustomerData(string name, string surname, string deliveryAddress)
    {
        WaitForCreatePurchasePage();

        _driver.FindElement(_purchaseName).Clear();
        _driver.FindElement(_purchaseName).SendKeys(name);

        _driver.FindElement(_purchaseSurname).Clear();
        _driver.FindElement(_purchaseSurname).SendKeys(surname);

        _driver.FindElement(_purchaseDeliveryAddress).Clear();
        _driver.FindElement(_purchaseDeliveryAddress).SendKeys(deliveryAddress);
    }

    public void SelectPaymentMethod(string paymentMethodValue)
    {
        WaitForBeingVisible(_paymentMethodSelected);
        var select = new SelectElement(_driver.FindElement(_paymentMethodSelected));
        select.SelectByValue(paymentMethodValue);
    }

    public void SavePurchase()
    {
        WaitForBeingClickable(_savePurchase);
        _driver.FindElement(_savePurchase).Click();
    }

    public string GetTotalQuantity()
    {
        WaitForBeingVisible(_createPurchaseTotalQuantity);
        return _driver.FindElement(_createPurchaseTotalQuantity).Text;
    }

    public string GetTotalPrice()
    {
        WaitForBeingVisible(_createPurchaseTotalPrice);
        return _driver.FindElement(_createPurchaseTotalPrice).Text;
    }

    public void WaitForCreatePurchasePage()
    {
        WaitForBeingVisible(_purchaseName);
        WaitForBeingVisible(_purchaseSurname);
        WaitForBeingVisible(_purchaseDeliveryAddress);
        WaitForBeingVisible(_paymentMethodSelected);
    }

    public void SavePurchaseAndWaitForDetail()
    {
        WaitForBeingClickable(_savePurchase);
        _driver.FindElement(_savePurchase).Click();

        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));
        wait.Until(d => d.Url.ToLower().Contains("detailpurchase"));
    }

    public void ModifyCartAndWaitForSelection()
    {
        WaitForBeingClickable(_modifyPurchaseCart);
        _driver.FindElement(_modifyPurchaseCart).Click();

        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));
        wait.Until(d => d.FindElements(By.Id("TableOfDevices")).Any());
    }
}
