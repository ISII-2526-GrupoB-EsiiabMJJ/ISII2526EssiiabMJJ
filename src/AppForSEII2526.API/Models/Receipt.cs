using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AppForSEII2526.API.Models
{
    public class Receipt
    {
        public Receipt() { }

        public Receipt(string customerNameSurname, string deliveryAddress,
                       PaymentMethodTypes paymentMethod, DateTime receiptDate, decimal totalPrice, ApplicationUser applicationUser)
        {
            CustomerNameSurname = customerNameSurname;
            DeliveryAddress = deliveryAddress;
            PaymentMethod = paymentMethod;
            ReceiptDate = receiptDate;
            TotalPrice = totalPrice;
            ApplicationUser = applicationUser;
            ApplicationUserId = applicationUser.Id;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string CustomerNameSurname { get; set; }

        [Required]
        [StringLength(250)]
        public string DeliveryAddress { get; set; }

        [Required]
        public PaymentMethodTypes PaymentMethod { get; set; }

        [Required]
        public DateTime ReceiptDate { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total price must be greater than 0.")]
        public decimal TotalPrice { get; set; }

        public IList<ReceiptItem> ReceiptItems { get; set; } = new List<ReceiptItem>();

        public ApplicationUser ApplicationUser { get; set; }
        
        [Required]
        public string ApplicationUserId { get; set; }
        public override bool Equals(object obj)
        {
            if (obj is not Receipt other)
                return false;

            return Id == other.Id;
        }
    }
}
