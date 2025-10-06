using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Security.Cryptography.X509Certificates;

namespace AppForSEII2526.API.Models
{
    public class Model
    {

        public Model(){}
        public Model(int id, string nameModel)
        {
            this.Id = id;
            this.NameModel = nameModel;
        }

        public bool Equals(object? obj)
        {
            if (obj is Model model)
            {
                return this.Id.Equals(model.Id);
            }
            return false;
        }

        public override int GetHashCode() {
            return HashCode.Combine(Id, NameModel);
        }

        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MinLength(2, ErrorMessage = "El nombre del modelo debe tener mínimo 2 caracteres")]
        public string NameModel { get; set; }

    }
}
