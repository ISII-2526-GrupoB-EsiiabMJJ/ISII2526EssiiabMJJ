using AppForSEII2526.Web.API;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace AppForSEII2526.Web
{
    public class ReviewStateContainer
    {
        //we use
        public CreateReviewDTO Review { get; private set; } = new CreateReviewDTO()
        {
            ReviewItems = new List<ReviewItemDTO>(),
            DateOfReview = DateTime.Now

        };

        public event Action? OnChange;

        private void NotifyStateChanged() => OnChange?.Invoke();


        public void AddDeviceForReview(ReviewItemDTO movie)
        {
            if (!Review.ReviewItems.Any(ri => ri.DeviceId == movie.DeviceId))
                Review.ReviewItems.Add(new ReviewItemDTO()
                {
                    DeviceId = movie.DeviceId,
                    Comment = movie.Comment,
                    Model = movie.Model,
                    Name = movie.Name,
                    Rating = movie.Rating,
                    Year = movie.Year
                }
            );
            ComputeTotalPrice();
        }

        private void ComputeTotalPrice()
        {
            int x = 0;
        }
        /*
        public void RemoveDeviceFromReview(ReviewItemDTO item)
        {
            Review.ReviewItems.Remove(item);
            ComputeTotalPrice();
        }
        */
        public void RemoveDeviceFromReview(int deviceId)
        {
            var itemToRemove = Review.ReviewItems.FirstOrDefault(item => item.DeviceId == deviceId);
            if (itemToRemove != null)
            {
                Review.ReviewItems.Remove(itemToRemove);
                ComputeTotalPrice();
            }
        }


        public void ClearDeviceList()
        {
            Review.ReviewItems.Clear();
        }

        public void ReviewProcessed()
        {
            //we have finished the rental process so we create a new object without data
            Review = new CreateReviewDTO()
            {
                ReviewItems = new List<ReviewItemDTO>()
            };
        }

    }


}
