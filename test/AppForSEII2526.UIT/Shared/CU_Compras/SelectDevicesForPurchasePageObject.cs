using OpenQA.Selenium;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.Shared.CU_Compras;

public class SelectDevicesForPurchasePageObject : PageObject
{
    private readonly By _searchDevices = By.Id("searchDevices");
    private readonly By _selectColor = By.Id("selectColor");
    private readonly By _searchDevicesButton = By.Id("searchDevicesButton");
    private readonly By _clearDeviceFiltersButton = By.Id("clearDeviceFiltersButton");
    private readonly By _tableOfDevices = By.Id("TableOfDevices");
    private readonly By _purchaseDevicesButton = By.Id("purchaseDevicesButton");
    private readonly By _emptyCartMessage = By.Id("emptyCartMessage");
    private readonly By _cartTotalQuantity = By.Id("cartTotalQuantity");
    private readonly By _cartTotalPrice = By.Id("cartTotalPrice");

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

    public void ClearFilters()
    {
        WaitForBeingClickable(_clearDeviceFiltersButton);
        _driver.FindElement(_clearDeviceFiltersButton).Click();
    }

    public void AddDevice(string deviceName)
    {
        var addDeviceButton = By.Id($"deviceToBuy_{deviceName}");
        WaitForBeingClickable(addDeviceButton);
        _driver.FindElement(addDeviceButton).Click();
    }

    public void RemoveDevice(int deviceId)
    {
        var removeDeviceButton = By.Id($"removeDevice_{deviceId}");
        WaitForBeingClickable(removeDeviceButton);
        _driver.FindElement(removeDeviceButton).Click();
    }

    public void IncreaseQuantity(int deviceId)
    {
        var increaseQuantityButton = By.Id($"increaseQuantity_{deviceId}");
        WaitForBeingClickable(increaseQuantityButton);
        _driver.FindElement(increaseQuantityButton).Click();
    }

    public void DecreaseQuantity(int deviceId)
    {
        var decreaseQuantityButton = By.Id($"decreaseQuantity_{deviceId}");
        WaitForBeingClickable(decreaseQuantityButton);
        _driver.FindElement(decreaseQuantityButton).Click();
    }

    public void ContinuePurchase()
    {
        WaitForBeingClickable(_purchaseDevicesButton);
        _driver.FindElement(_purchaseDevicesButton).Click();
    }

    public bool IsDevicesTableVisible()
    {
        return _driver.FindElements(_tableOfDevices).Any();
    }

    public bool IsEmptyCartMessageVisible()
    {
        return _driver.FindElements(_emptyCartMessage).Any();
    }

    public string GetCartTotalQuantity()
    {
        WaitForBeingVisible(_cartTotalQuantity);
        return _driver.FindElement(_cartTotalQuantity).Text;
    }

    public string GetCartTotalPrice()
    {
        WaitForBeingVisible(_cartTotalPrice);
        return _driver.FindElement(_cartTotalPrice).Text;
    }
}