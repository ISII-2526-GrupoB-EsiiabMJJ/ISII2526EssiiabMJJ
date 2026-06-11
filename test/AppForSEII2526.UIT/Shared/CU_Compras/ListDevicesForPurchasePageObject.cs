using OpenQA.Selenium;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.Shared.CU_Compras;

public class ListDevicesForPurchasePageObject : PageObject
{
    // Campos y botones principales de la pantalla de selección de dispositivos.
    private readonly By _searchDevices = By.Id("searchDevices");
    private readonly By _selectColor = By.Id("selectColor");
    private readonly By _searchDevicesButton = By.Id("searchDevicesButton");
    private readonly By _purchaseDevicesButton = By.Id("purchaseDevicesButton");
    private readonly By _emptyCartMessage = By.Id("emptyCartMessage");

    public ListDevicesForPurchasePageObject(IWebDriver driver, ITestOutputHelper output)
        : base(driver, output)
    {
    }

    public void SearchByName(string name)
    {
        // Se escribe el nombre del dispositivo que se quiere buscar.
        WaitForBeingVisible(_searchDevices);

        var searchInput = _driver.FindElement(_searchDevices);
        searchInput.Clear();
        searchInput.SendKeys(name);
        searchInput.SendKeys(Keys.Tab);

        // Se lanza la búsqueda y se espera a que la tabla o el mensaje de vacío se actualicen.
        WaitForBeingClickable(_searchDevicesButton);
        _driver.FindElement(_searchDevicesButton).Click();

        WaitForSearchToFinish();
    }

    public void SearchByColor(string color)
    {
        // Se escribe el color por el que se quiere filtrar el listado.
        WaitForBeingVisible(_selectColor);

        var colorInput = _driver.FindElement(_selectColor);
        colorInput.Clear();
        colorInput.SendKeys(color);
        colorInput.SendKeys(Keys.Tab);

        // Se aplica el filtro y se espera a que se refresquen los resultados.
        WaitForBeingClickable(_searchDevicesButton);
        _driver.FindElement(_searchDevicesButton).Click();

        WaitForSearchToFinish();
    }

    public void SearchByNameAndColor(string name, string color)
    {
        // Se completan los dos filtros disponibles: nombre y color.
        WaitForBeingVisible(_searchDevices);

        var searchInput = _driver.FindElement(_searchDevices);
        searchInput.Clear();
        searchInput.SendKeys(name);
        searchInput.SendKeys(Keys.Tab);

        var colorInput = _driver.FindElement(_selectColor);
        colorInput.Clear();
        colorInput.SendKeys(color);
        colorInput.SendKeys(Keys.Tab);

        // Se ejecuta la búsqueda combinada.
        WaitForBeingClickable(_searchDevicesButton);
        _driver.FindElement(_searchDevicesButton).Click();

        WaitForSearchToFinish();
    }

    public void AddDevice(string deviceName)
    {
        // Cada botón de añadir se identifica con el nombre del dispositivo.
        // Esto permite seleccionar un dispositivo concreto dentro de la tabla.
        var addDeviceButton = By.Id($"deviceToBuy_{deviceName}");

        WaitForBeingClickable(addDeviceButton);
        _driver.FindElement(addDeviceButton).Click();
    }

    public void ContinuePurchase()
    {
        // Avanza desde el carrito de selección al formulario de compra.
        WaitForBeingClickable(_purchaseDevicesButton);
        _driver.FindElement(_purchaseDevicesButton).Click();
    }

    public bool IsEmptyCartMessageVisible()
    {
        // Indica si la pantalla está mostrando el mensaje de carrito vacío.
        return _driver.FindElements(_emptyCartMessage).Any();
    }

    public void RemoveFirstDeviceFromCart()
    {
        // Elimina el primer dispositivo que aparece en el carrito.
        // Se usa el prefijo del id porque cada botón de eliminar pertenece a un dispositivo distinto.
        var removeButton = By.CssSelector("button[id^='removeDevice_']");

        WaitForBeingClickable(removeButton);
        _driver.FindElement(removeButton).Click();
    }

    public void RemoveDevicesFromCart()
    {
        // Elimina el primer dispositivo que aparece en el carrito.
        // Se usa el prefijo del id porque cada botón de eliminar pertenece a un dispositivo distinto.
        var clearCart = By.CssSelector("button[id^='clearDevicesCar']");

        WaitForBeingClickable(clearCart);
        _driver.FindElement(clearCart).Click();
    }

    public void IncreaseFirstDeviceQuantity()
    {
        // Aumenta la cantidad del primer dispositivo seleccionado en el carrito.
        var increaseButton = By.CssSelector("button[id^='increaseQuantity_']");

        WaitForBeingClickable(increaseButton);
        _driver.FindElement(increaseButton).Click();
    }

    private void WaitForSearchToFinish()
    {
        // Tras aplicar un filtro, la página puede mostrar una tabla de resultados
        // o un mensaje indicando que no hay dispositivos disponibles.
        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));

        wait.Until(d =>
            d.FindElements(By.Id("TableOfDevices")).Any(e => e.Displayed) ||
            d.FindElements(By.Id("noDevicesMessage")).Any(e => e.Displayed));
    }
}