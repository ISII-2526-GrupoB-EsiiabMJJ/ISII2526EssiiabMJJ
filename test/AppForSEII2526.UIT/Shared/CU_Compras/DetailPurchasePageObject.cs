using OpenQA.Selenium;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.Shared.CU_Compras;

public class DetailPurchasePageObject : PageObject
{
    private readonly By _purchaseDetail = By.Id("purchaseDetail");
    private readonly By _purchaseCustomerName = By.Id("purchaseCustomerName");
    private readonly By _purchaseCustomerSurname = By.Id("purchaseCustomerSurname");
    private readonly By _purchaseDeliveryAddress = By.Id("purchaseDeliveryAddress");
    private readonly By _purchaseTotalQuantity = By.Id("purchaseTotalQuantity");
    private readonly By _purchaseTotalPrice = By.Id("purchaseTotalPrice");
    private readonly By _purchaseDetailItemsTable = By.Id("PurchaseDetailItemsTable");

    public DetailPurchasePageObject(IWebDriver driver, ITestOutputHelper output)
        : base(driver, output)
    {
    }

    public bool IsLoaded()
    {
        return _driver.FindElements(_purchaseDetail).Any();
    }

    public string GetCustomerName()
    {
        WaitForBeingVisible(_purchaseCustomerName);
        return _driver.FindElement(_purchaseCustomerName).Text;
    }

    public string GetCustomerSurname()
    {
        WaitForBeingVisible(_purchaseCustomerSurname);
        return _driver.FindElement(_purchaseCustomerSurname).Text;
    }

    public string GetDeliveryAddress()
    {
        WaitForBeingVisible(_purchaseDeliveryAddress);
        return _driver.FindElement(_purchaseDeliveryAddress).Text;
    }

    public string GetTotalQuantity()
    {
        WaitForBeingVisible(_purchaseTotalQuantity);
        return _driver.FindElement(_purchaseTotalQuantity).Text;
    }

    public string GetTotalPrice()
    {
        WaitForBeingVisible(_purchaseTotalPrice);
        return _driver.FindElement(_purchaseTotalPrice).Text;
    }

    public bool CheckListOfPurchasedDevices(List<string[]> expectedPurchaseItems)
    {
        return CheckBodyTable(expectedPurchaseItems, _purchaseDetailItemsTable);
    }

}