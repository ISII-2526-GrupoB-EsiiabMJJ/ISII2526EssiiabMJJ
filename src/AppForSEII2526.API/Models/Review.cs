namespace AppForSEII2526.API.Models
{
    public class Review
    {
        public Review()
        {

        }

        public Review(
            int reviewId,
            int customerId,
            int overallRating,

            string reviewTitle,
            string customerCountry,
            string customerName,

            ApplicationUser applicationUser,
            DateTime dateOfReview,
            IList<ReviewItem> reviewItems
        )
        {
            this.ReviewId = reviewId;
            this.CustomerId = customerId;
            this.OverallRating = overallRating;

            this.ReviewTitle = reviewTitle;
            this.CustomerName = customerName;
            this.CustomerCountry = customerCountry;

            this.DateOfReview = dateOfReview;

            this.ReviewItems = reviewItems;

            this.ApplicationUser = applicationUser;
            this.ApplicationUserId = applicationUser.Id;
        }

        [Key]
        public int ReviewId { get; set; }


        [Required]
        public int CustomerId { get; set; }
        [Required]
        [Range(0, 5, ErrorMessage = "La puntuación debe estar entre 0 y 5 estrellas")]
        public int OverallRating { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 10, ErrorMessage = "Debe tener entre 10 y 100 caracteres el titulo de la reseña")]
        public string ReviewTitle { get; set; }
        [Required]

        [StringLength(100, MinimumLength = 2, ErrorMessage = "Debe tener entre 2 y 40 caracteres el nombre del país")]
        public string CustomerCountry { get; set; }

        public string? CustomerName { get; set; }

        public DateTime DateOfReview { get; set; }
        public IList<ReviewItem> ReviewItems { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public string ApplicationUserId { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is Review review)
            {
                return this.ReviewId.Equals(review.ReviewId);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                this.ReviewId,
                this.CustomerId,
                this.OverallRating,
                this.ReviewTitle,
                this.CustomerCountry,
                this.CustomerName,
                this.DateOfReview
            );
        }
    }
}
