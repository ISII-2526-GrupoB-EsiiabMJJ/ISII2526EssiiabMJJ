using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppForSEII2526.API.Models
{
    public class ReceiptItem
    {
        public ReceiptItem() { }

        public ReceiptItem(string model, int receiptId, int repairId)
        {
            Model = model;
            ReceiptId = receiptId;
            RepairId = repairId;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Model { get; set; }

        [Required]
        public int ReceiptId { get; set; }

        [ForeignKey(nameof(ReceiptId))]
        public Receipt Receipt { get; set; }

        [Required]
        public int RepairId { get; set; }

        [ForeignKey(nameof(RepairId))]
        public Repair Repair { get; set; }
        public override bool Equals(object obj)
        {
            if (obj is not ReceiptItem other)
                return false;

            return Id == other.Id;
        }

        public override int GetHashCode() => Id.GetHashCode();
    }
}
