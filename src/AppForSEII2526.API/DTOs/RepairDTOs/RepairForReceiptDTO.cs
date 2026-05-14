using System.ComponentModel.DataAnnotations;

namespace AppForSEII2526.API.DTOs.RepairDTOs
{
    public class RepairForReceiptDTO
    {
        public RepairForReceiptDTO()
        {
        }

        public RepairForReceiptDTO(int id, string name, string description, decimal cost, string scale)
        {
            Id = id;
            Name = name;
            Description = description;
            Cost = cost;
            Scale = scale;
        }

        public int Id { get; set; }

        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(300)]
        public string Description { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue)]
        public decimal Cost { get; set; }

        [StringLength(50)]
        public string Scale { get; set; } = string.Empty;
    }
}
