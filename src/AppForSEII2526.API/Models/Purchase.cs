using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppForSEII2526.API.Models
{
    public class Purchase
    {
        public Purchase() { Items = new List<PurchaseItem>(); }

        public Purchase(
            string customerUserName,
            string customerNameSurname,
            ApplicationUser applicationUser,
            string deliveryAddress,
            DateTime purchaseDateUtc,
            IList<PurchaseItem> items,
            PaymentMethod paymentMethod)
        {
            ApplicationUser = applicationUser ?? throw new ArgumentNullException(nameof(applicationUser));

            CustomerUserName = customerUserName;
            CustomerNameSurname = customerNameSurname;
            DeliveryAddress = deliveryAddress;
            PurchaseDateUtc = purchaseDateUtc;
            Items = items ?? new List<PurchaseItem>();
            ApplicationUserId = applicationUser.Id;
            PaymentMethod = paymentMethod;

        }

        public Purchase(
            int id,
            string customerUserName,
            string customerNameSurname,
            ApplicationUser applicationUser,
            string deliveryAddress,
            DateTime purchaseDateUtc,
            IList<PurchaseItem> items,
            PaymentMethod paymentMethod)
            : this(customerUserName, customerNameSurname, applicationUser, deliveryAddress, purchaseDateUtc, items, paymentMethod)
        {
            Id = id;
        }

        [Key] public int Id { get; set; }

        [Required] public string CustomerUserName { get; set; } = string.Empty;
        [Required] public string CustomerNameSurname { get; set; } = string.Empty;

        [Required] public string ApplicationUserId { get; set; } = string.Empty;

        public ApplicationUser ApplicationUser { get; set; } = default!;

        [Required, MaxLength(250)] public string DeliveryAddress { get; set; } = string.Empty;
        [Required] public PaymentMethod PaymentMethod { get; set; }
        [Required] public DateTime PurchaseDateUtc { get; set; } = DateTime.UtcNow;

        [Required, Column(TypeName = "decimal(12,2)")] public decimal TotalPrice { get; set; }
        [Required] public int TotalQuantity { get; set; }

        public IList<PurchaseItem> Items { get; set; } = new List<PurchaseItem>();

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (obj is not Purchase other) return false;
            if (Id == 0 || other.Id == 0) return false;
            return Id == other.Id;
        }
        public override int GetHashCode() => Id.GetHashCode();
    }

    public enum PaymentMethod { CreditCard, PayPal, Cash }
}
