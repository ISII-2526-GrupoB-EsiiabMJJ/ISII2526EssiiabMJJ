using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AppForMovies.UIT.CU_Reseñas
{
    public class CreateReview_PO : PageObject
    {
        private IWebElement _customerName() => _driver.FindElement(By.Id("CustomerName"));
        private IWebElement _customerCountry() => _driver.FindElement(By.Id("CustomerCountry"));
        private IWebElement _reviewTitle() => _driver.FindElement(By.Id("ReviewTitle"));




        public CreateReview_PO(IWebDriver driver, ITestOutputHelper output)
            : base(driver, output)
        {
        }

        public void FillInReviewInfo(string? customerName, string customerCountry, string reviewTitle)
        {
            WaitForBeingVisible(By.Id("CustomerName"));
            _customerName().SendKeys(customerName);
            WaitForBeingVisible(By.Id("CustomerCountry"));
            _customerCountry().SendKeys(customerCountry);
            WaitForBeingVisible(By.Id("ReviewTitle"));
            _reviewTitle().SendKeys(reviewTitle);
        }
        public bool CheckCreateReviewPersistance(
            string? customerName,
            string customerCountry,
            string reviewTitle)
        {
            bool equals = true;
            WaitForBeingVisible(By.Id("CustomerName"));
            equals = equals && customerName ==_customerName().GetAttribute("value");
            WaitForBeingVisible(By.Id("CustomerCountry"));
            equals = equals && customerCountry == _customerCountry().GetAttribute("value");
            WaitForBeingVisible(By.Id("ReviewTitle"));
            equals = equals && reviewTitle == _reviewTitle().GetAttribute("value");

            return equals;
        }

        public void FillDeviceRating(string deviceRating, string deviceId)
        {
            var inputRating = _driver.FindElement(By.Id(deviceId));
            inputRating.Clear();
            inputRating.SendKeys(deviceRating.ToString());
        }

        public void FillDeviceComment(string deviceComment, string deviceId)
        {
            _driver.FindElement(By.Id(deviceId)).SendKeys(deviceComment);
        }


        public void PressReviewYourDevices()
        {
            _driver.FindElement(By.Id("endReview")).Click();
        }

        public void PressModifyDevices()
        {
            _driver.FindElement(By.Id("ModifyDevices")).Click();
        }

        public bool CheckListOfRentalItems(List<string[]> expectedReviewItems)
        {
            return CheckBodyTable(expectedReviewItems, By.Id("TableOfReviewItems"));
        }

        public bool CheckValidationError(string expectedError)
        {
            return _driver.PageSource.Contains(expectedError);
        }

        public List<string[]> reviewItemsInfo() {
            var listToReturn = new List<string[]>();
            //identify the table
            var table = _driver.FindElement(By.Id("TableBodyOfReviewItems"));
            //extract its rows
            var rows = table.FindElements(By.TagName("tr"));

            for (int i=0; i<rows.Count; i++) {
                var cells = rows[i].FindElements(By.TagName("td"));
                var extractedRowInfo = new List<string>();

                foreach(var cell in cells) {
                    var input = cell.FindElements(By.TagName("input")).FirstOrDefault();
                    if (input != null) {
                        extractedRowInfo.Add(input.GetAttribute("value") ?? string.Empty);
                    }
                    else
                    {
                        extractedRowInfo.Add(cell.Text.Trim());
                    }
                }
                listToReturn.Add(extractedRowInfo.ToArray());

            }
            return listToReturn;
        }

    }
}
