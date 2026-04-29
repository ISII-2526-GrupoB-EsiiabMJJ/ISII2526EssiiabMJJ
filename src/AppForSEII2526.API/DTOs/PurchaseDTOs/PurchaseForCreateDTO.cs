using AppForSEII2526.API.Models;
using System.ComponentModel.DataAnnotations;

namespace AppForSEII2526.API.DTOs.PurchaseDTOs
{
    public class PurchaseForCreateDTO
    {
        public PurchaseForCreateDTO(
            string customerUserName,
            string name,
            string surname,
            string deliveryAddress,
            PaymentMethod paymentMethod,
            IList<PurchaseItemDTO> purchaseItems)
        {
            CustomerUserName = customerUserName;
            Name = name;
            Surname = surname;
            DeliveryAddress = deliveryAddress;
            PaymentMethod = paymentMethod;
            PurchaseItems = purchaseItems;
        }

        [Required]
        public string CustomerUserName { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Surname { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 5)]
        public string DeliveryAddress { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        [Required]
        public IList<PurchaseItemDTO> PurchaseItems { get; set; }
    }
}