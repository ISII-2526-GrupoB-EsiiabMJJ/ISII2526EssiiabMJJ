using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppForSEII2526.API.Models
{
    public class Repair
    {
        public Repair() { }

        public Repair(string name, string description, decimal cost, int scaleId)
        {
            Name = name;
            Description = description;
            Cost = cost;
            ScaleId = scaleId;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Repair name cannot be longer than 100 characters.")]
        public string Name { get; set; }

        [Required]
        [StringLength(300)]
        public string Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Cost must be greater than 0.")]
        public decimal Cost { get; set; }

        [Required]
        public int ScaleId { get; set; }

        [ForeignKey(nameof(ScaleId))]
        public Scale Scale { get; set; }

        public IList<ReceiptItem> ReceiptItems { get; set; } = new List<ReceiptItem>();
    }
}
