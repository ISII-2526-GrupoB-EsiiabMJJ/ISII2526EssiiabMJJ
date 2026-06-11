using System.ComponentModel.DataAnnotations;

namespace AppForSEII2526.API.DTOs.PurchaseDTOs
{
    public class PurchaseDetailDTO
    {
        public PurchaseDetailDTO(
            int id,
            string name,
            string surname,
            string deliveryAddress,
            DateTime purchaseDateUtc,
            double totalPrice,
            int totalQuantity,
            //PaymentMethod paymentMethod,
            IList<PurchaseItemDTO> purchaseItems)
        {
            Id = id;
            Name = name;
            Surname = surname;
            DeliveryAddress = deliveryAddress;
            PurchaseDateUtc = purchaseDateUtc;
            TotalPrice = totalPrice;
            TotalQuantity = totalQuantity;
            PurchaseItems = purchaseItems;
        }

        public int Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(100)]
        public string Surname { get; set; }

        [StringLength(200)]
        public string DeliveryAddress { get; set; }

        public DateTime PurchaseDateUtc { get; set; }

        public double TotalPrice { get; set; }

        public int TotalQuantity { get; set; }

        public IList<PurchaseItemDTO> PurchaseItems { get; set; }
    }
}