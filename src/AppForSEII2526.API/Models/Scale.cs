using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AppForSEII2526.API.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class Scale
    {
        public Scale() { }

        public Scale(string name)
        {
            Name = name;
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Scale name cannot be longer than 50 characters.", MinimumLength = 3)]
        public string Name { get; set; }

        public IList<Repair> Repairs { get; set; } = new List<Repair>();
    }
}
