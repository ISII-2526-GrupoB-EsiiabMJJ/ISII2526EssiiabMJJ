using AppForSEII2526.API.Models;
using System.ComponentModel.DataAnnotations;

namespace AppForSEII2526.API.DTOs.RepairDTOs
{
    public class ReceiptDetailDTO
    {
        public ReceiptDetailDTO()
        {
            ReceiptItems = new List<ReceiptItemDTO>();
        }

        public ReceiptDetailDTO(
            int id,
            string customerNameSurname,
            string deliveryAddress,
            PaymentMethodTypes paymentMethod,
            DateTime receiptDate,
            decimal totalPrice,
            IList<ReceiptItemDTO> receiptItems)
        {
            Id = id;
            CustomerNameSurname = customerNameSurname;
            DeliveryAddress = deliveryAddress;
            PaymentMethod = paymentMethod;
            ReceiptDate = receiptDate;
            TotalPrice = totalPrice;
            ReceiptItems = receiptItems;
        }

        public int Id { get; set; }

        [StringLength(150)]
        public string CustomerNameSurname { get; set; } = string.Empty;

        [StringLength(250)]
        public string DeliveryAddress { get; set; } = string.Empty;

        public PaymentMethodTypes PaymentMethod { get; set; }

        public DateTime ReceiptDate { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal TotalPrice { get; set; }

        public IList<ReceiptItemDTO> ReceiptItems { get; set; }
    }
}