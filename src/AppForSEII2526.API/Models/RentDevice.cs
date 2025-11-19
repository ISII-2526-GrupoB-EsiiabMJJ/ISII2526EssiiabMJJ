using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppForSEII2526.API.Models
{
    public class RentDevice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
       

        public Device Device { get; set; }
        public int DeviceId { get; set; }

        public Rental Rent { get; set; }
        public int RentalId { get; set; }  // 🔹 Foreign key (descomenta esta línea)

        [StringLength(100, ErrorMessage = "Title name cannot be longer than 50 characters.")]
        public string? Description { get; set; }

        [Required]
        public double Price { get; set; }

        [Required]
        public int Quantity { get; set; }

        public RentDevice() { }

        public RentDevice(Device device, Rental rent)
        {
            Device = device;
            DeviceId = device.Id;
            Rent = rent;
            RentalId = rent.Id;
        }

        public RentDevice(int deviceId, Rental rental, double price, int quantity)
        {
            DeviceId = deviceId; 
            Rent = rental;
            Price = price;
            Quantity = quantity;
        }

        

        public override bool Equals(object? obj)
        {
            return obj is RentDevice item &&
                   EqualityComparer<Device>.Default.Equals(Device, item.Device) &&
                   Price == item.Price &&
                   Quantity == item.Quantity &&
                   Description == item.Description &&
                   DeviceId == item.DeviceId;
        }
    }
}
