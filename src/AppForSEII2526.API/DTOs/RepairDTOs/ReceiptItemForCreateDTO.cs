using System.ComponentModel.DataAnnotations;

namespace AppForSEII2526.API.DTOs.RepairDTOs
{
    public class ReceiptItemForCreateDTO
    {
        public ReceiptItemForCreateDTO()
        {
        }

        public ReceiptItemForCreateDTO(string model, int repairId)
        {
            Model = model;
            RepairId = repairId;
        }

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Model { get; set; } = string.Empty;

        [Required]
        public int RepairId { get; set; }
    }
}