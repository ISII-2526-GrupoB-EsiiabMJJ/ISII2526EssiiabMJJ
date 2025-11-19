namespace AppForSEII2526.API.DTOs.ReviewDTOs
{
    public class ReviewItemDTO
    {
        public ReviewItemDTO() { }

        public ReviewItemDTO(
            int id,
            int year,
            int rating,
            string name,
            string model,
            string comment)
        {
            DeviceId = id;
            Year = year;
            Rating = rating;
            Name = name;
            Model = model;
            Comment = comment;
        }
        public int DeviceId { get; set; }
        public int Year { get; set; }
        public int Rating { get; set; }
        public string Name { get; set; }
        public string Model { get; set; }
        public string Comment { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is ReviewItemDTO reviewItemDTO)
            {
                return
                    DeviceId.Equals(reviewItemDTO.DeviceId) &&
                    Year.Equals(reviewItemDTO.Year) &&
                    Rating.Equals(reviewItemDTO.Rating) &&
                    Model.Equals(reviewItemDTO.Model) &&
                    Comment.Equals(reviewItemDTO.Comment);
            }
            return false;
        }
    }
}