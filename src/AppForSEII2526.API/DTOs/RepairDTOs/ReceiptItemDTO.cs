using System.ComponentModel.DataAnnotations;

namespace AppForSEII2526.API.DTOs.RepairDTOs
{
    public class ReceiptItemDTO
    {
        public ReceiptItemDTO()
        {
        }

        public ReceiptItemDTO(
            string model,
            int repairId,
            string repairName,
            string repairDescription,
            decimal repairCost,
            string scale)
        {
            Model = model;
            RepairId = repairId;
            RepairName = repairName;
            RepairDescription = repairDescription;
            RepairCost = repairCost;
            Scale = scale;
        }

        [StringLength(100)]
        public string Model { get; set; } = string.Empty;

        public int RepairId { get; set; }

        [StringLength(100)]
        public string RepairName { get; set; } = string.Empty;

        [StringLength(300)]
        public string RepairDescription { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue)]
        public decimal RepairCost { get; set; }

        [StringLength(50)]
        public string Scale { get; set; } = string.Empty;
    }
}