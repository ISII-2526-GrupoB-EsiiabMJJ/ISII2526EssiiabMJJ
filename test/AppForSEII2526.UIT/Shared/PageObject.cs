using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForSEII2526.UIT.Shared
{
    public class PageObject
    {
        protected IWebDriver _driver;

        // Permite escribir información en la salida del Explorador de Pruebas.
        protected readonly ITestOutputHelper _output;

        // Elementos comunes de los diálogos modales usados en la aplicación.
        private By _modalTitle = By.ClassName("modal-title");
        private By _modalBody = By.ClassName("modal-body");
        private By _okModalDialog = By.Id("Button_DialogOK");

        protected PageObject(IWebDriver driver, ITestOutputHelper output)
        {
            _driver = driver;
            this._output = output;
        }

        public void InputDateInDatePicker(By datepicker, DateTime date)
        {
            // Se localiza el input de fecha y se escribe la fecha por partes.
            IWebElement webElement = _driver.FindElement(datepicker);

            var action = new Actions(_driver);

            webElement.Clear();
            webElement.Click();

            // Día.
            action.KeyDown(Keys.Left).Perform();
            action.KeyDown(Keys.Left).Perform();
            action.SendKeys(date.ToString("dd")).Perform();

            // Mes.
            action.KeyDown(Keys.Left).Perform();
            action.KeyDown(Keys.Left).Perform();
            action.KeyDown(Keys.Right).Perform();
            action.SendKeys(date.ToString("MM")).Perform();

            // Año.
            action.KeyDown(Keys.Right).Perform();
            action.KeyDown(Keys.Right).Perform();
            action.SendKeys(date.ToString("yyyy")).Perform();
        }

        public bool CheckBodyTable(List<string[]> expectedRows, By IdTable)
        {
            string expectedRow, actualRow;
            int i, j;
            bool result = true;

            // Se espera a que la tabla esté visible antes de leer sus filas.
            WaitForBeingVisible(IdTable);

            IList<IWebElement> actualrows = _driver
                .FindElement(IdTable)
                .FindElement(By.TagName("tbody"))
                .FindElements(By.TagName("tr"))
                .ToList();

            // La tabla debe tener el mismo número de filas que se esperan en el test.
            if (actualrows.Count != expectedRows.Count)
            {
                _output.WriteLine(
                    $"Error: \n Expected number of rows:{expectedRows.Count} \n Actual number of rows:{actualrows.Count}");

                return false;
            }

            for (i = 0; i < expectedRows.Count; i++)
            {
                // Cada array de expectedRows representa los textos esperados para una fila.
                // Se unen los valores en el mismo orden en el que aparecen en la tabla.
                expectedRow = expectedRows[i][0];

                for (j = 1; j < expectedRows[i].Count(); j++)
                    expectedRow = expectedRow + " " + expectedRows[i][j];

                // Se obtiene el texto real de la fila correspondiente.
                actualRow = actualrows
                    .Select(m => m.Text)
                    .ToList()[i];

                // Se utiliza StartsWith porque la tabla puede mostrar columnas adicionales
                // después de los datos principales que se están comprobando.
                if (!actualRow.StartsWith(expectedRow))
                {
                    _output.WriteLine(
                        $"Error: \n \t expected row:{expectedRow} \n \t actual row:{actualRow}");

                    result = false;
                }
            }

            return result;
        }

        public bool CheckModalBodyText(string expectedBody, By modal)
        {
            // Se espera a que el modal esté visible antes de leer el mensaje.
            WaitForBeingVisible(modal);

            var actualBody = _driver.FindElement(_modalBody).Text;

            return actualBody.Contains(expectedBody);
        }

        public bool CheckModalTitleText(string expectedTitle, By modal)
        {
            // Se espera a que el modal esté visible antes de leer el título.
            WaitForBeingVisible(modal);

            var actualTitle = _driver.FindElement(_modalTitle).Text;

            return actualTitle.Contains(expectedTitle);
        }

        public void PressOkModalDialog()
        {
            // Se pulsa el botón de confirmación del modal cuando esté disponible.
            WaitForBeingVisible(_okModalDialog);
            _driver.FindElement(_okModalDialog).Click();
        }

        public void WaitForBeingClickable(By IdElement)
        {
            // Espera explícita para acciones que requieren que el elemento pueda pulsarse.
            var wait = new WebDriverWait(_driver, new TimeSpan(0, 0, 30));

            wait.Until(ExpectedConditions.ElementToBeClickable(IdElement));
        }

        public void WaitForBeingVisible(By IdElement)
        {
            // Espera explícita para asegurar que el elemento ya está visible en la página.
            var wait = new WebDriverWait(_driver, new TimeSpan(0, 0, 30));

            wait.Until(ExpectedConditions.ElementIsVisible(IdElement));
        }

        public void WaitForBeingVisibleIgnoringExeptionTypes(By IdElement)
        {
            // Espera más tolerante para elementos que pueden tardar más o aparecer tras recargas parciales.
            var wait = new WebDriverWait(_driver, new TimeSpan(0, 10, 0));

            wait.IgnoreExceptionTypes(
                typeof(NoSuchElementException),
                typeof(WebDriverTimeoutException),
                typeof(UnhandledAlertException),
                typeof(ElementClickInterceptedException));

            bool notFoundButton = true;

            while (notFoundButton)
            {
                try
                {
                    wait.Until(ExpectedConditions.ElementIsVisible(IdElement));
                    notFoundButton = false;
                }
                catch (ElementClickInterceptedException ex)
                {
                    _output.WriteLine(ex.Message);
                }
            }
        }

        public void WaitForTextToBePresentInElement(By IdElement, string expectedText)
        {
            // Espera a que un elemento ya visible contenga el texto esperado.
            var wait = new WebDriverWait(_driver, new TimeSpan(0, 0, 30));

            IWebElement element = _driver.FindElement(IdElement);

            wait.Until(ExpectedConditions.TextToBePresentInElement(element, expectedText));
        }

        public void ImplicitWait(int seconds)
        {
            // Espera implícita general para búsquedas de elementos en la página.
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(seconds);
        }
    }
}