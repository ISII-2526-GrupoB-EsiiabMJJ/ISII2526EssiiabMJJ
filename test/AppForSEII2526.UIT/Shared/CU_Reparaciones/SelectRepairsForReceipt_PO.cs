using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit.Abstractions;

namespace AppForSEII2526.UIT.Shared.CU_Reparaciones
{
    public class SelectRepairsForReceipt_PO : PageObject
    {
        private readonly By _repairNameFilter = By.Id("repairNameFilter");
        private readonly By _scaleFilter = By.Id("scaleFilter");
        private readonly By _searchRepairsButton = By.Id("searchRepairsButton");
        private readonly By _clearRepairFiltersButton = By.Id("clearRepairFiltersButton");
        private readonly By _repairCartTotalPrice = By.Id("repairCartTotalPrice");
        private readonly By _continueReceiptButton = By.Id("continueReceiptButton");

        public SelectRepairsForReceipt_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }

        public void WaitPageReady()
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));

            wait.IgnoreExceptionTypes(
                typeof(StaleElementReferenceException),
                typeof(NoSuchElementException));

            wait.Until(d =>
                ExistsById("repairNameFilter") &&
                (
                    ExistsById("TableOfRepairs") ||
                    PageContains("no hay reparaciones disponibles")
                ));
        }

        public void FilterRepairs(string name, string scale)
        {
            WaitPageReady();

            SafeSetInputValue("repairNameFilter", name);
            SafeSetInputValue("scaleFilter", scale);

            SafeClick(_searchRepairsButton);

            WaitAfterSearch();
        }

        public void ClearFilters()
        {
            SafeClick(_clearRepairFiltersButton);
            WaitAfterSearch();
        }

        public void AddFirstVisibleRepair()
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));

            wait.IgnoreExceptionTypes(
                typeof(StaleElementReferenceException),
                typeof(NoSuchElementException));

            wait.Until(d => d.FindElements(By.CssSelector("button[id^='repairToAdd_']")).Any());

            SafeClickFirst(By.CssSelector("button[id^='repairToAdd_']"));

            wait.Until(d => ExistsById("RepairCartTable"));
        }

        public void RemoveFirstRepairFromCart()
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));

            wait.IgnoreExceptionTypes(
                typeof(StaleElementReferenceException),
                typeof(NoSuchElementException));

            wait.Until(d => d.FindElements(By.CssSelector("button[id^='repairToRemove_']")).Any());

            SafeClickFirst(By.CssSelector("button[id^='repairToRemove_']"));

            wait.Until(d => ExistsById("emptyRepairCartMessage"));
        }

        public void ContinueReceipt()
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));

            wait.IgnoreExceptionTypes(
                typeof(StaleElementReferenceException),
                typeof(NoSuchElementException),
                typeof(ElementClickInterceptedException));

            wait.Until(d => ExistsById("continueReceiptButton") && !IsButtonDisabledById("continueReceiptButton"));

            SafeClick(_continueReceiptButton);
        }

        public bool IsRepairsTableVisible()
        {
            return WaitUntilElementExistsById("TableOfRepairs", 10);
        }

        public bool IsCartTableVisible()
        {
            return WaitUntilElementExistsById("RepairCartTable", 10);
        }

        public bool IsEmptyCartMessageVisible()
        {
            return WaitUntilElementExistsById("emptyRepairCartMessage", 10);
        }

        public bool IsContinueButtonDisabled()
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

            wait.IgnoreExceptionTypes(
                typeof(StaleElementReferenceException),
                typeof(NoSuchElementException));

            wait.Until(d => ExistsById("continueReceiptButton"));

            return IsButtonDisabledById("continueReceiptButton");
        }

        public string GetCartTotalPrice()
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

            wait.IgnoreExceptionTypes(
                typeof(StaleElementReferenceException),
                typeof(NoSuchElementException));

            return wait.Until(d =>
            {
                try
                {
                    if (!ExistsById("repairCartTotalPrice"))
                    {
                        return string.Empty;
                    }

                    var text = (string)((IJavaScriptExecutor)_driver).ExecuteScript(
                        "return document.getElementById(arguments[0]).innerText;",
                        "repairCartTotalPrice");

                    return text ?? string.Empty;
                }
                catch
                {
                    return string.Empty;
                }
            });
        }

        public bool HasNoRepairsMessage()
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

            wait.IgnoreExceptionTypes(
                typeof(StaleElementReferenceException),
                typeof(NoSuchElementException));

            try
            {
                return wait.Until(d => PageContains("no hay reparaciones disponibles"));
            }
            catch
            {
                return false;
            }
        }

        private void WaitAfterSearch()
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));

            wait.IgnoreExceptionTypes(
                typeof(StaleElementReferenceException),
                typeof(NoSuchElementException));

            wait.Until(d =>
                ExistsById("TableOfRepairs") ||
                PageContains("no hay reparaciones disponibles"));
        }

        private bool WaitUntilElementExistsById(string id, int seconds)
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(seconds));

            wait.IgnoreExceptionTypes(
                typeof(StaleElementReferenceException),
                typeof(NoSuchElementException));

            try
            {
                return wait.Until(d => ExistsById(id));
            }
            catch
            {
                return false;
            }
        }

        private void SafeSetInputValue(string elementId, string? value)
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));

            wait.IgnoreExceptionTypes(
                typeof(StaleElementReferenceException),
                typeof(NoSuchElementException));

            wait.Until(d =>
            {
                try
                {
                    var script = @"
                        const element = document.getElementById(arguments[0]);

                        if (element === null) {
                            return false;
                        }

                        element.value = arguments[1];

                        element.dispatchEvent(new Event('input', { bubbles: true }));
                        element.dispatchEvent(new Event('change', { bubbles: true }));

                        return true;
                    ";

                    return (bool)((IJavaScriptExecutor)_driver).ExecuteScript(
                        script,
                        elementId,
                        value ?? string.Empty);
                }
                catch
                {
                    return false;
                }
            });
        }

        private bool ExistsById(string id)
        {
            try
            {
                var script = @"
                    const element = document.getElementById(arguments[0]);
                    return element !== null;
                ";

                return (bool)((IJavaScriptExecutor)_driver).ExecuteScript(script, id);
            }
            catch
            {
                return false;
            }
        }

        private bool IsButtonDisabledById(string id)
        {
            try
            {
                var script = @"
                    const element = document.getElementById(arguments[0]);

                    if (element === null) {
                        return true;
                    }

                    return element.disabled === true || element.hasAttribute('disabled');
                ";

                return (bool)((IJavaScriptExecutor)_driver).ExecuteScript(script, id);
            }
            catch
            {
                return true;
            }
        }

        private bool PageContains(string text)
        {
            try
            {
                return _driver.PageSource.ToLower().Contains(text.ToLower());
            }
            catch
            {
                return false;
            }
        }

        private void SafeClick(By by)
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));

            wait.IgnoreExceptionTypes(
                typeof(StaleElementReferenceException),
                typeof(NoSuchElementException),
                typeof(ElementClickInterceptedException));

            wait.Until(d =>
            {
                try
                {
                    var element = d.FindElement(by);

                    ScrollToElement(element);

                    if (!element.Displayed || !element.Enabled)
                    {
                        return false;
                    }

                    try
                    {
                        element.Click();
                    }
                    catch
                    {
                        ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", element);
                    }

                    return true;
                }
                catch
                {
                    return false;
                }
            });
        }

        private void SafeClickFirst(By by)
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));

            wait.IgnoreExceptionTypes(
                typeof(StaleElementReferenceException),
                typeof(NoSuchElementException),
                typeof(ElementClickInterceptedException));

            wait.Until(d =>
            {
                try
                {
                    var elements = d.FindElements(by)
                        .Where(e =>
                        {
                            try
                            {
                                return e.Displayed && e.Enabled;
                            }
                            catch
                            {
                                return false;
                            }
                        })
                        .ToList();

                    if (!elements.Any())
                    {
                        return false;
                    }

                    var element = elements.First();

                    ScrollToElement(element);

                    try
                    {
                        element.Click();
                    }
                    catch
                    {
                        ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].click();", element);
                    }

                    return true;
                }
                catch
                {
                    return false;
                }
            });
        }

        private void ScrollToElement(IWebElement element)
        {
            try
            {
                ((IJavaScriptExecutor)_driver).ExecuteScript(
                    "arguments[0].scrollIntoView({block: 'center'});",
                    element);

                Thread.Sleep(250);
            }
            catch
            {
                // Blazor puede repintar el elemento; el wait lo reintentará.
            }
        }
    }
}