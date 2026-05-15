using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.Shared.CU_Reparaciones
{
    public class CreateReceipt_PO : PageObject
    {
        private readonly By _customerNameSurname = By.Id("customerNameSurname");
        private readonly By _deliveryAddress = By.Id("deliveryAddress");
        private readonly By _paymentMethod = By.Id("paymentMethod");
        private readonly By _saveReceiptButton = By.Id("saveReceiptButton");
        private readonly By _modifyReceiptCart = By.Id("modifyReceiptCart");
        private readonly By _receiptValidationErrors = By.Id("receiptValidationErrors");
        private readonly By _tableOfReceiptItems = By.Id("TableOfReceiptItems");

        public CreateReceipt_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }

        public void WaitPageReady()
        {
            WaitForBeingVisible(_customerNameSurname);
        }

        public void FillFirstRepairModel(string model)
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));

            var modelInput = wait.Until(d =>
            {
                var inputs = d.FindElements(By.CssSelector("input[id^='repairModel_']"))
                    .Where(i => i.Displayed)
                    .ToList();

                return inputs.FirstOrDefault();
            });

            modelInput.Clear();
            modelInput.SendKeys(model);
        }

        public void FillCustomerData(string customerNameSurname, string deliveryAddress, string paymentMethodText)
        {
            WaitForBeingVisible(_customerNameSurname);

            var nameInput = _driver.FindElement(_customerNameSurname);
            nameInput.Clear();

            if (!string.IsNullOrWhiteSpace(customerNameSurname))
            {
                nameInput.SendKeys(customerNameSurname);
            }

            var addressInput = _driver.FindElement(_deliveryAddress);
            addressInput.Clear();

            if (!string.IsNullOrWhiteSpace(deliveryAddress))
            {
                addressInput.SendKeys(deliveryAddress);
            }

            if (!string.IsNullOrWhiteSpace(paymentMethodText))
            {
                var select = new SelectElement(_driver.FindElement(_paymentMethod));
                select.SelectByText(paymentMethodText);
            }
        }

        public void SaveReceipt()
        {
            WaitForBeingClickable(_saveReceiptButton);
            _driver.FindElement(_saveReceiptButton).Click();
        }

        public void ModifyRepairs()
        {
            WaitForBeingClickable(_modifyReceiptCart);
            _driver.FindElement(_modifyReceiptCart).Click();
        }

        public bool IsReceiptItemsTableVisible()
        {
            return _driver.FindElements(_tableOfReceiptItems).Any();
        }

        public bool HasValidationErrors()
        {
            return _driver.FindElements(_receiptValidationErrors).Any();
        }
    }
}