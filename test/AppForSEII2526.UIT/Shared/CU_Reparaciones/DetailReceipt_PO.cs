using OpenQA.Selenium;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.Shared.CU_Reparaciones
{
    public class DetailReceipt_PO : PageObject
    {
        private readonly By _receiptDetail = By.Id("receiptDetail");
        private readonly By _receiptCustomerName = By.Id("receiptCustomerName");
        private readonly By _receiptDeliveryAddress = By.Id("receiptDeliveryAddress");
        private readonly By _receiptPaymentMethod = By.Id("receiptPaymentMethod");
        private readonly By _receiptTotalPrice = By.Id("receiptTotalPrice");
        private readonly By _receiptItemsTable = By.Id("ReceiptItemsTable");
        private readonly By _newReceiptButton = By.Id("newReceiptButton");

        public DetailReceipt_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }

        public bool IsLoaded()
        {
            WaitForBeingVisible(_receiptDetail);
            return _driver.FindElements(_receiptDetail).Any();
        }

        public bool CheckReceiptDetail(string customerNameSurname, string deliveryAddress, string paymentMethod)
        {
            WaitForBeingVisible(_receiptDetail);

            bool result = true;

            result = result && _driver.FindElement(_receiptCustomerName).Text.Contains(customerNameSurname);
            result = result && _driver.FindElement(_receiptDeliveryAddress).Text.Contains(deliveryAddress);
            result = result && _driver.FindElement(_receiptPaymentMethod).Text.Contains(paymentMethod);
            result = result && _driver.FindElement(_receiptTotalPrice).Text.Contains("€");

            return result;
        }

        public bool IsItemsTableVisible()
        {
            return _driver.FindElements(_receiptItemsTable).Any();
        }

        public void StartNewReceipt()
        {
            WaitForBeingClickable(_newReceiptButton);
            _driver.FindElement(_newReceiptButton).Click();
        }
    }
}