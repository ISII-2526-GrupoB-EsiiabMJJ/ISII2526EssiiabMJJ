using System.ComponentModel;
using System.Drawing;
using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace AppForSEII2526.API.Models
{
    public class Device
    {
        public Device()
        {

        }

        public Device(
            int id,
            int year,
            QualityType quality,
            int quantityForRent,
            int quantityForPurchase,

            double priceForRent,
            double priceForPurchase,

            string name,
            string color,
            string brand,
            string description,

            Model model,

            IList<RentDevice> rentedDevices,
            IList<ReviewItem> reviewItems,
            IList<PurchaseItem> purchaseItems)
        {
            this.Id = id;
            this.Year = year;
            this.Quality = quality;
            this.quantityForRent = quantityForRent;
            this.quantityForPurchase = quantityForPurchase;

            this.priceForRent = priceForRent;
            this.priceForPurchase = priceForPurchase;

            this.Name = name;
            this.Color = color;
            this.Brand = brand;
            this.Description = description;

            this.Model = model;

            this.RentedDevices = rentedDevices;
            this.ReviewItems = reviewItems;
            this.PurchaseItems = purchaseItems;
        }

       
        public int Id { get; set; }

        [ForeignKey("ModelId")]
        public int ModelId { get; set; }
        public Model Model { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public QualityType Quality { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "El numero de dispositivos para alquilar debe ser mayor que 0")]
        public int quantityForRent { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "El numero de dispositivos para vender debe ser mayor que 0")]
        public int quantityForPurchase { get; set; }


        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "El precio para alquilar debe ser minimo 1")]
        public double priceForRent { get; set; }
        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "El precio para vender debe ser minimo 1")]
        public double priceForPurchase { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre minimo debe tener 3 caracteres y como maximo 100")]
        public string Name { get; set; }
        [Required]
        public string Color { get; set; }
        [Required]
        public string Brand { get; set; }
        [Required]
        public string Description { get; set; }



        //[Required]
        public IList<ReviewItem> ReviewItems { get; set; }
        //[Required]
        public IList<RentDevice> RentedDevices { get; set; }
        //[Required]
        public IList<PurchaseItem> PurchaseItems { get; set; }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name);
        }

        public override bool Equals(object? obj)
        {
            if (obj is Device device)
            {
                return this.Id.Equals(device.Id);
            }
            return false;
        }
    }
}