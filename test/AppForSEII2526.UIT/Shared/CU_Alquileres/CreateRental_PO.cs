using OpenQA.Selenium.Support.UI;

namespace AppForSEII2526.UIT.Shared.CU_Alquileres
{
    public class CreateRental_PO : PageObject
    {
        private IWebElement _nameSurname() => _driver.FindElement(By.Id("NameSurname"));
        private IWebElement _deliveryAddress() => _driver.FindElement(By.Id("DeliveryAddress"));
        private IWebElement _paymentMethod() => _driver.FindElement(By.Id("PaymentMethod"));

        public CreateRental_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output) { }

       
        public void FillInRentalInfo(string nameSurname, string deliveryAddress,
                              string paymentMethod, string quantity = "1")
        {
            WaitForBeingVisible(By.Id("NameSurname"));

            
            var nameField = _driver.FindElement(By.Id("NameSurname"));
            nameField.Clear();
            if (!string.IsNullOrEmpty(nameSurname))
                nameField.SendKeys(nameSurname);

            var addressField = _driver.FindElement(By.Id("DeliveryAddress"));
            addressField.Clear();
            if (!string.IsNullOrEmpty(deliveryAddress))
                addressField.SendKeys(deliveryAddress);

           
            if (!string.IsNullOrEmpty(paymentMethod))
            {
                var selectElement = new SelectElement(_paymentMethod());
                selectElement.SelectByText(paymentMethod);
            }

           
            var quantityFields = _driver.FindElements(By.CssSelector("input[id^='quantity_']"));
            foreach (var qField in quantityFields)
            {
                qField.Clear();
                if (!string.IsNullOrEmpty(quantity))
                    qField.SendKeys(quantity);
            }
        }

        public void FillInRentalDescription(string rentalDescription, string deviceName)  
        {
            WaitForBeingVisible(By.Id($"description_{deviceName}"));
            _driver.FindElement(By.Id($"description_{deviceName}")).SendKeys(rentalDescription);
        }

       
        public void PressRentYourDevices()
        {
           
            WaitForBeingClickable(By.Id("Submit"));
            _driver.FindElement(By.Id("Submit")).Click();
        }

        public void PressModifyDevices()
        {
            WaitForBeingClickable(By.Id("ModifyDevices"));
            _driver.FindElement(By.Id("ModifyDevices")).Click();
        }

       
        public void PressOkModalDialog()
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".modal-dialog")));

            var okButton = _driver.FindElement(By.Id("Button_DialogOK"));
            okButton.Click();
        }

        public bool CheckListOfRentalItems(List<string[]> expectedRentalItems)
        {
            return CheckBodyTable(expectedRentalItems, By.Id("TableOfRentalItems"));  
        }

        public bool CheckValidationError(string expectedError)
        {
            return _driver.PageSource.Contains(expectedError);
        }

       
        public bool CheckCreateRentalPersistance(string nameSurname, string deliveryAddress, string paymentMethod)
        {
            bool equals = true;
            equals = equals && nameSurname == _nameSurname().GetAttribute("value");
            equals = equals && deliveryAddress == _deliveryAddress().GetAttribute("value");

            var selectElement = new SelectElement(_paymentMethod());
            equals = equals && paymentMethod == selectElement.SelectedOption.Text;

            return equals;
        }
    }
}