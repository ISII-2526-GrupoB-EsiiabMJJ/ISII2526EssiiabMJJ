using OpenQA.Selenium;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.Shared.CU_Compras;

public class DetailPurchasePageObject : PageObject
{
    // Contenedor principal de la pantalla de detalle de compra.
    private readonly By _purchaseDetail = By.Id("purchaseDetail");

    // Datos personales mostrados en el detalle.
    private readonly By _purchaseCustomerName = By.Id("purchaseCustomerName");
    private readonly By _purchaseCustomerSurname = By.Id("purchaseCustomerSurname");
    private readonly By _purchaseDeliveryAddress = By.Id("purchaseDeliveryAddress");

    // Resumen de cantidad y precio total de la compra.
    private readonly By _purchaseTotalQuantity = By.Id("purchaseTotalQuantity");
    private readonly By _purchaseTotalPrice = By.Id("purchaseTotalPrice");

    // Tabla donde se muestran los dispositivos incluidos en la compra.
    private readonly By _purchaseDetailItemsTable = By.Id("PurchaseDetailItemsTable");

    public DetailPurchasePageObject(IWebDriver driver, ITestOutputHelper output)
        : base(driver, output)
    {
    }

    public bool IsLoaded()
    {
        // La pantalla se considera cargada si existe el contenedor principal del detalle.
        return _driver.FindElements(_purchaseDetail).Any();
    }

    public string GetCustomerName()
    {
        // Obtiene el nombre del cliente mostrado en el detalle de compra.
        WaitForBeingVisible(_purchaseCustomerName);
        return _driver.FindElement(_purchaseCustomerName).Text;
    }

    public string GetCustomerSurname()
    {
        // Obtiene los apellidos del cliente mostrados en el detalle de compra.
        WaitForBeingVisible(_purchaseCustomerSurname);
        return _driver.FindElement(_purchaseCustomerSurname).Text;
    }

    public string GetDeliveryAddress()
    {
        // Obtiene la dirección de entrega asociada a la compra.
        WaitForBeingVisible(_purchaseDeliveryAddress);
        return _driver.FindElement(_purchaseDeliveryAddress).Text;
    }

    public string GetTotalQuantity()
    {
        // Obtiene el texto donde se muestra la cantidad total de unidades compradas.
        WaitForBeingVisible(_purchaseTotalQuantity);
        return _driver.FindElement(_purchaseTotalQuantity).Text;
    }

    public string GetTotalPrice()
    {
        // Obtiene el texto donde se muestra el importe total de la compra.
        WaitForBeingVisible(_purchaseTotalPrice);
        return _driver.FindElement(_purchaseTotalPrice).Text;
    }

    public bool CheckListOfPurchasedDevices(List<string[]> expectedPurchaseItems)
    {
        // Comprueba que la tabla del detalle contiene los dispositivos esperados.
        // Cada array representa una fila esperada con los datos relevantes del dispositivo.
        return CheckBodyTable(expectedPurchaseItems, _purchaseDetailItemsTable);
    }
}