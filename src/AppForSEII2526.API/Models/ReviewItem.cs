namespace AppForSEII2526.API.Models
{
    public class ReviewItem
    {
        public ReviewItem() { }

        public ReviewItem(
            int id,
            int rating,
            string comment,
            Device device,
            Review review)
        {
            this.Rating = rating;
            this.Comment = comment;
            this.Device = device;
            this.DeviceId = device.Id;
            this.Review = review;
            this.ReviewId = review.ReviewId;
        }

        public int Id { get; set; }

        [Required]
        [Range(0, 5, ErrorMessage = "La puntuación debe estar entre 0 y 5 estrellas")]
        public int Rating { get; set; }
        [Required]
        public string Comment { get; set; }

        [ForeignKey("DeviceId")]
        public Device Device { get; set; }
        [Required]
        public int DeviceId { get; set; }

        [ForeignKey("ReviewId")]
        public Review Review { get; set; }
        [Required]
        public int ReviewId { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is ReviewItem reviewItem && this.Id.Equals(reviewItem.Id);
        }
    }
};