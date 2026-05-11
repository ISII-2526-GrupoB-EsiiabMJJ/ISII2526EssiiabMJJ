using AppForSEII2526.API.Models;
using System.ComponentModel.DataAnnotations;

namespace AppForSEII2526.API.DTOs.RepairDTOs
{
    public class ReceiptForCreateDTO
    {
        public ReceiptForCreateDTO()
        {
            ReceiptItems = new List<ReceiptItemForCreateDTO>();
        }

        public ReceiptForCreateDTO(
            string customerUserName,
            string customerNameSurname,
            string deliveryAddress,
            PaymentMethodTypes paymentMethod,
            IList<ReceiptItemForCreateDTO> receiptItems)
        {
            CustomerUserName = customerUserName;
            CustomerNameSurname = customerNameSurname;
            DeliveryAddress = deliveryAddress;
            PaymentMethod = paymentMethod;
            ReceiptItems = receiptItems;
        }

        [Required]
        public string CustomerUserName { get; set; } = string.Empty;

        [Required]
        [StringLength(150, MinimumLength = 3)]
        public string CustomerNameSurname { get; set; } = string.Empty;

        [Required]
        [StringLength(250, MinimumLength = 5)]
        public string DeliveryAddress { get; set; } = string.Empty;

        [Required]
        public PaymentMethodTypes PaymentMethod { get; set; }

        [Required]
        public IList<ReceiptItemForCreateDTO> ReceiptItems { get; set; }
    }
}