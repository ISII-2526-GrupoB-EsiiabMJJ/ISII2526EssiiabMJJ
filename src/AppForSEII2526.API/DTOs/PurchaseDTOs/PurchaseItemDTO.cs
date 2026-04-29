using System.ComponentModel.DataAnnotations;
namespace AppForSEII2526.API.DTOs.PurchaseDTOs
{
    public class PurchaseItemDTO
    {
        public PurchaseItemDTO(int deviceId, string brand, string model, string color, decimal price, int quantity, string? description)
        {
            DeviceId = deviceId;
            Brand = brand;
            Model = model;
            Color = color;
            Price = price;
            Quantity = quantity;
            Description = description;
        }

        public int DeviceId { get; set; }

        [StringLength(50)]
        public string Brand { get; set; }

        [StringLength(50)]
        public string Model { get; set; }

        [StringLength(50)]
        public string Color { get; set; }

        [Range(0.0, 1000.0)]
        public decimal Price { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "You must provide a quantity higher than 0")]
        public int Quantity { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }
    }
}