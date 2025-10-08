using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AppForSEII2526.API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser() { }

        public ApplicationUser(string id, string name, string surname, string userName)
        {
            Id = id;
            Name = name;
            Surname = surname;
            UserName = userName;
            Email = userName;
        }

        [Display(Name = "Name")]
        public string? Name { get; set; }

        [Display(Name = "Surname")]
        public string? Surname { get; set; }

        public ICollection<Receipt> Receipt { get; set; } = new List<Receipt>();
    }
}