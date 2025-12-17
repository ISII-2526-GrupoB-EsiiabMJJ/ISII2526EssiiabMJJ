using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII2526.UIT.CU_Reseñas
{
    public class ListDeviceForReview_PO : PageObject{

        private By inputBrand = By.Id("inputBrand");
        private By inputYear = By.Id("inputYear");
        private By searchButton = By.Id("searchButton");
        private By devicesTableBy = By.Id("devicesTable");
        private By noDevicesAvailableBy = By.Id("noDevicesAvailable");


        public ListDeviceForReview_PO(IWebDriver driver, ITestOutputHelper output) :  base(driver, output) {
        }

        public void SearchDevices(string brand, string year)
        {
            // Escribir la marca
            WaitForBeingVisible(inputBrand);
            WaitForBeingClickable(inputBrand);
            var brandInput = _driver.FindElement(inputBrand);
            brandInput.Clear();
            brandInput.SendKeys(brand);

            // Escribir el año
            WaitForBeingVisible(inputYear);
            WaitForBeingClickable(inputYear);
            var yearInput = _driver.FindElement(inputYear);
            yearInput.Clear();
            yearInput.SendKeys(year);

            // Hacer clic en el botón de búsqueda
            WaitForBeingClickable(searchButton);
            _driver.FindElement(searchButton).Click();

            // Esperar a que aparezca la tabla o el mensaje
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(driver =>
                driver.FindElements(devicesTableBy).Any() ||
                driver.FindElements(noDevicesAvailableBy).Any()
            );
        }


        public bool CheckListOfDevices(List<string[]> expectedDevices)
        {
            var rows = _driver.FindElements(By.CssSelector("#devicesTable tbody tr"));

            if (rows.Count != expectedDevices.Count)
                return false;

            for (int i = 0; i < rows.Count; i++)
            {
                var cells = rows[i].FindElements(By.TagName("td"));

                // Solo las primeras 5 columnas
                string name  = cells[0].Text.Trim();
                string brand = cells[1].Text.Trim();
                string color = cells[2].Text.Trim();
                string year  = cells[3].Text.Trim();
                string model = cells[4].Text.Trim();

                if (name  != expectedDevices[i][0] ||
                    brand != expectedDevices[i][1] ||
                    color != expectedDevices[i][2] ||
                    year  != expectedDevices[i][3] ||
                    model != expectedDevices[i][4]
                    )
                {
                    return false;
                }
            }

            //return CheckBodyTable(expectedMovies, devicesTableBy);
            return true;
        }

        public void clickButton(By locator) {
            WaitForBeingVisible(locator);
            WaitForBeingClickable(locator);
            _driver.FindElement(locator).Click();
            //var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            //esperar hastta que sea visible
            //wait.Until(ExpectedConditions.ElementIsVisible(By.Id(buttonId)));
            //var button = _driver.FindElements(By.Id(buttonId));

        }
        public string GetTextById(By locator)
        {
            // Esperar a que el contador sea visible
            //WaitForBeingVisible(By.Id("devicesCounter"));
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));
            var element = wait.Until(ExpectedConditions.ElementIsVisible(locator));
            // Leer el texto
            return _driver.FindElement(locator).Text;
        }

        public bool checkClicableButton(By locator) {
            
            WaitForBeingVisible(locator);
            var button = _driver.FindElement(locator);
            return button.Displayed && button.Enabled;

        }

    }
}
