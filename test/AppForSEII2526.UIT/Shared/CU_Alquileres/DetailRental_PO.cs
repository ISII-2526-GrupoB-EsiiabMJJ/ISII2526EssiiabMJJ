namespace AppForSEII2526.UIT.Shared.CU_Alquileres
{
    public class DetailRental_PO : PageObject
    {
        public DetailRental_PO(IWebDriver driver, ITestOutputHelper output) : base(driver, output) { }

        public bool CheckRentalDetail(string nameSurname, string deliveryAddress, string paymentMethod,
    DateTime rentalDate, DateTime from, DateTime to, string totalPrice)
        {
            WaitForBeingVisible(By.Id("CustomerName"));
            bool result = true;

            result = result && _driver.FindElement(By.Id("CustomerName")).Text.Contains(nameSurname);
            result = result && _driver.FindElement(By.Id("DeliveryAddress")).Text.Contains(deliveryAddress);
            result = result && _driver.FindElement(By.Id("PaymentMethod")).Text.Contains(paymentMethod);
            result = result && _driver.FindElement(By.Id("TotalPrice")).Text.Contains(totalPrice);

            
            try
            {
                var actualRentalDateText = _driver.FindElement(By.Id("RentalDate")).Text;
                var rentalPeriodText = _driver.FindElement(By.Id("RentalPeriod")).Text;

                result = result && rentalPeriodText.Contains(from.ToString("dd/MM")) &&
                                    rentalPeriodText.Contains(to.ToString("dd/MM"));
            }
            catch { }

            return result;
        }

        public bool CheckListOfDevices(List<string[]> expectedRentalItems)
        {
            return CheckBodyTable(expectedRentalItems, By.Id("RentedDevices"));  
        }
    }
}