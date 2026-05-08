using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AppForSEII2526.UIT.Shared.CU_Alquileres
{
    public class ListDevicesForRental_PO : PageObject
    {
        
        private By _deviceModelBy = By.Id("deviceModel");
        private By _maxPriceBy = By.Id("maxRentalPrice");
        private By _fromBy = By.Id("fromDate");
        private By _toBy = By.Id("toDate");
        private By _searchDevicesBy = By.Id("searchDevices");
        private By _rentButtonBy = By.Id("Rent");
        private By _tableOfDevicesBy = By.Id("devicesTable");
        private By _noDevicesAvailableBy = By.TagName("em");

        public ListDevicesForRental_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output) { }

        private string ToSafeId(string value)
        {
            return value
                .Replace(" ", "")
                .Replace(".", "")
                .Replace(",", "")
                .Trim();
        }

        
        public void FilterDevices(string modelFilter, string maxPrice, DateTime from, DateTime to)
        {
            WaitPageReady();

            
            if (!string.IsNullOrEmpty(modelFilter))
            {
                WaitForBeingVisible(_deviceModelBy);

                var modelInput = _driver.FindElement(_deviceModelBy);
                modelInput.Clear();
                modelInput.SendKeys(modelFilter);
            }

            
            if (!string.IsNullOrEmpty(maxPrice))
            {
                WaitForBeingVisible(_maxPriceBy);

                var priceInput = _driver.FindElement(_maxPriceBy);
                priceInput.Clear();
                priceInput.SendKeys(maxPrice.Replace(",", "."));
            }

           
            InputDateInDatePicker(_fromBy, from);
            InputDateInDatePicker(_toBy, to);

            
            WaitForBeingClickable(_searchDevicesBy);
            _driver.FindElement(_searchDevicesBy).Click();

          
            WaitForDevicesTable();

            Thread.Sleep(1500);
        }

        public void WaitPageReady()
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));
        }

        public void WaitForTextToBePresentInElement(By locator, string expectedText)
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(d => {
                try
                {
                    var element = d.FindElement(locator);
                    return element.Displayed && element.Text.Contains(expectedText);
                }
                catch { return false; }
            });
        }

        public void FilterDevicesWithPrice(string maxPrice)
        {
            FilterDevices("", maxPrice, DateTime.Today.AddDays(1), DateTime.Today.AddDays(2));
        }

        public bool CheckListOfDevices(List<string[]> expectedDevices)
        {
            return CheckBodyTable(expectedDevices, _tableOfDevicesBy);
        }

        public void clickButton(By locator)
        {
            WaitForBeingClickable(locator);
            _driver.FindElement(locator).Click();
        }

        public bool checkClicableButton(By locator)
        {
            try
            {
                WaitForBeingVisible(locator);
                var button = _driver.FindElement(locator);
                return button.Displayed && button.Enabled;
            }
            catch
            {
                return false;
            }
        }

        public string GetTextById(By locator)
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            var element = wait.Until(ExpectedConditions.ElementIsVisible(locator));
            return element.Text;
        }
       

        /// <summary>
        /// Click en botón ADD usando el MODEL correcto del dispositivo
        /// ID real: "deviceToAdd_NVIDIA GeForce RTX 5090"
        /// </summary>
        /// 


      
        public void ClickAddDevice(string deviceModel)
        {
            var addButtonId = $"deviceToAdd_{deviceModel}";

            _output.WriteLine($"Click ADD: {addButtonId}");

            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

            var button = wait.Until(d =>
            {
                try
                {
                    var el = d.FindElement(By.Id(addButtonId));
                    return el.Displayed && el.Enabled ? el : null;
                }
                catch
                {
                    return null;
                }
            });

            button.Click();
        }

        /// <summary>
        /// Click en botón REMOVE usando el MODEL correcto
        /// </summary>
       
        public void ClickRemoveDevice(string deviceModel)
        {
            var removeButtonId = $"deviceToRemove_{deviceModel}";

            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

            var button = wait.Until(d =>
            {
                try
                {
                    var el = d.FindElement(By.Id(removeButtonId));
                    return el.Displayed && el.Enabled ? el : null;
                }
                catch
                {
                    return null;
                }
            });

            button.Click();
        }

        /// <summary>
        /// DEBUG: Lista todos los dispositivos disponibles
        /// </summary>
        public List<string> GetAvailableDevices()
        {
            var devices = new List<string>();
            try
            {
                var rows = _driver.FindElements(By.CssSelector("#devicesTable tbody tr"));
                _output.WriteLine($"Encontradas {rows.Count} filas de dispositivos");

                foreach (var row in rows)
                {
                    var addButtons = row.FindElements(By.CssSelector("button[id^='deviceToAdd_']"));
                    if (addButtons.Count > 0)
                    {
                        var id = addButtons[0].GetAttribute("id");
                        var model = id?.Replace("deviceToAdd_", "");
                        if (!string.IsNullOrEmpty(model))
                        {
                            devices.Add(model);
                            _output.WriteLine($"Dispositivo: {model}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _output.WriteLine($"Error listando dispositivos: {ex.Message}");
            }
            return devices;
        }

        /// <summary>
        /// Espera a que la tabla esté visible o mensaje de no dispositivos
        /// </summary>
        public void WaitForDevicesTable()
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(d => d.FindElements(_tableOfDevicesBy).Count > 0 ||
                           d.FindElements(_noDevicesAvailableBy).Count > 0);
        }

        public void WaitForDevicesPage()
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

            wait.Until(d =>
                d.FindElements(By.Id("fromDate")).Count > 0 &&
                d.FindElements(By.Id("devicesTable")).Count > 0
            );
        }

    }

}