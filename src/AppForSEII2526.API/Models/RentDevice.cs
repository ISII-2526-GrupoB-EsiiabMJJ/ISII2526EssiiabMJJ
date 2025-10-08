namespace AppForSEII2526.API.Models
{
    public class RentDevice
    {
        public RentDevice()
        {
        }
        public RentDevice(Device device, Rental rent)
        {
            Device = device;
            DeviceId = device.Id;
            Rent = rent;
            RentalId = rent.Id;
        }
        public RentDevice(Device device, Rental rent, string? description) : this(device, rent)
        {
            Description = description;
            Price = device.priceForRent;
            Quantity = device.quantityForRent;
        }

        public RentDevice(int deviceId, Rental rental, double price, int quantity)
        {
            DeviceId = deviceId;
            Rent = rental;
            Price = price;
            Quantity = quantity;
        }
        public RentDevice(int deviceId, Rental rental, double price, int quantity, string? description) : this(deviceId, rental, price, quantity) => Description = description;

        public Device Device { get; set; }

        public int DeviceId { get; set; }


        public Rental Rent { get; set; }

        public int RentalId { get; set; }

        [StringLength(100, ErrorMessage = "Title name cannot be longer than 50 characters.")]
        public string? Description { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please, set your price for rental")]
        public double Price { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please, set your quantity for rental")]
        public int Quantity { get; set; }

        public override int GetHashCode()
        {
            return HashCode.Combine(DeviceId, RentalId);
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
