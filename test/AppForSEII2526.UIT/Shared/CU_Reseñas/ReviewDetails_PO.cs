using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppForMovies.UIT.CU_Reseñas
{
    public class ReviewDetails_PO : PageObject
    {
        public ReviewDetails_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output)
        {
        }

        public bool CheckReviewDetail(string? customerName, string customerCountry, string reviewTitle,
            List<string[]> expectedReviewed, By tableIdBy)
        {
            WaitForBeingVisible(By.Id("ReviwedDevices"));
            bool result = true;
            result = result && _driver.FindElement(By.Id("CustomerName")).Text.Contains(customerName);
            result = result && _driver.FindElement(By.Id("CustomerCountry")).Text.Contains(customerCountry);
            result = result && _driver.FindElement(By.Id("ReviewTitle")).Text.Contains(reviewTitle);

            result = result && CheckBodyTable(expectedReviewed, tableIdBy);
            return result;

        }

        public bool CheckListOfMovies(List<string[]> expectedRentalItems)
        {
            return CheckBodyTable(expectedRentalItems, By.Id("RentedMovies"));
        }
    }
}