using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII2526.API.Models
{
    [PrimaryKey(nameof(DeviceId), nameof(PurchaseId))]
    public class PurchaseItem
    {
        public PurchaseItem() { }

        public PurchaseItem(Device device, int quantity, Purchase purchase)
        {
            if (device is null) throw new ArgumentNullException(nameof(device));
            if (purchase is null) throw new ArgumentNullException(nameof(purchase));
            if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be > 0.");

            Device = device;
            DeviceId = device.Id;
            Purchase = purchase;
            PurchaseId = purchase.Id;

            Price = Convert.ToDecimal(device.priceForPurchase);
            Quantity = quantity;
        }

        public PurchaseItem(Device device, decimal unitPrice, int quantity, Purchase purchase)
            : this(device, quantity, purchase)
        {
            Price = unitPrice;
        }

        public Device Device { get; set; } = default!;
        public int DeviceId { get; set; }

        public Purchase Purchase { get; set; } = default!;
        public int PurchaseId { get; set; }

        [Precision(10, 2)]
        public decimal Price { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "You must provide a quantity higher than 0")]
        public int Quantity { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters")]
        public string? Description { get; set; }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (obj is not PurchaseItem other) return false;

            if (DeviceId == 0 || PurchaseId == 0 || other.DeviceId == 0 || other.PurchaseId == 0)
                return false;

            return DeviceId == other.DeviceId && PurchaseId == other.PurchaseId;
        }

        public override int GetHashCode() => HashCode.Combine(DeviceId, PurchaseId);
    }
}