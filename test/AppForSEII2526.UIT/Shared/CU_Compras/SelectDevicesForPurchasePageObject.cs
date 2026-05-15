using OpenQA.Selenium;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.Shared.CU_Compras;

public class SelectDevicesForPurchasePageObject : PageObject
{
    private readonly By _searchDevices = By.Id("searchDevices");
    private readonly By _selectColor = By.Id("selectColor");
    private readonly By _searchDevicesButton = By.Id("searchDevicesButton");
    private readonly By _purchaseDevicesButton = By.Id("purchaseDevicesButton");
    private readonly By _emptyCartMessage = By.Id("emptyCartMessage");

    public SelectDevicesForPurchasePageObject(IWebDriver driver, ITestOutputHelper output)
        : base(driver, output)
    {
    }

    public void SearchByName(string name)
    {
        WaitForBeingVisible(_searchDevices);
        _driver.FindElement(_searchDevices).Clear();
        _driver.FindElement(_searchDevices).SendKeys(name);
        _driver.FindElement(_searchDevicesButton).Click();
    }

    public void SearchByColor(string color)
    {
        WaitForBeingVisible(_selectColor);
        _driver.FindElement(_selectColor).Clear();
        _driver.FindElement(_selectColor).SendKeys(color);
        _driver.FindElement(_searchDevicesButton).Click();
    }

    public void SearchByNameAndColor(string name, string color)
    {
        WaitForBeingVisible(_searchDevices);
        _driver.FindElement(_searchDevices).Clear();
        _driver.FindElement(_searchDevices).SendKeys(name);

        _driver.FindElement(_selectColor).Clear();
        _driver.FindElement(_selectColor).SendKeys(color);

        _driver.FindElement(_searchDevicesButton).Click();
    }

    public void AddDevice(string deviceName)
    {
        var addDeviceButton = By.Id($"deviceToBuy_{deviceName}");
        WaitForBeingClickable(addDeviceButton);
        _driver.FindElement(addDeviceButton).Click();
    }

    public void ContinuePurchase()
    {
        WaitForBeingClickable(_purchaseDevicesButton);
        _driver.FindElement(_purchaseDevicesButton).Click();
    }

    public bool IsEmptyCartMessageVisible()
    {
        return _driver.FindElements(_emptyCartMessage).Any();
    }

    public void RemoveFirstDeviceFromCart()
    {
        var removeButton = By.CssSelector("button[id^='removeDevice_']");
        WaitForBeingClickable(removeButton);
        _driver.FindElement(removeButton).Click();
    }

    public void IncreaseFirstDeviceQuantity()
    {
        var increaseButton = By.CssSelector("button[id^='increaseQuantity_']");
        WaitForBeingClickable(increaseButton);
        _driver.FindElement(increaseButton).Click();
    }
}