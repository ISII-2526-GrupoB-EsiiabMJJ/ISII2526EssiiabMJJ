using System.ComponentModel.DataAnnotations;

namespace AppForSEII2526.API.DTOs.DeviceDTOs
{
    public class DeviceForPurchaseDTO   
    {
        public DeviceForPurchaseDTO(int id, string name, double price, string brand, string model, string color)
        {
            Id = id;
            Name = name;
            Price = price;
            Brand = brand;
            Model = model;
            Color = color;
        }

        public int Id { get; set; }

        [StringLength(50, ErrorMessage = "Name cannot be longer than 50 characters.", MinimumLength = 1)]
        public string Name { get; set; }

        [Range(0.0, 1000.0, ErrorMessage = "Price must be between 0 and 1000.")]
        public double Price { get; set; }

        [StringLength(50, ErrorMessage = "Brand cannot be longer than 50 characters.", MinimumLength = 1)]
        public string Brand { get; set; }

        [StringLength(50, ErrorMessage = "Model cannot be longer than 50 characters.", MinimumLength = 1)]
        public string Model { get; set; }

        [StringLength(50, ErrorMessage = "Color cannot be longer than 50 characters.", MinimumLength = 1)]
        public string Color { get; set; }
    }
}