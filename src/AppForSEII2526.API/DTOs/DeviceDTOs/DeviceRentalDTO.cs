using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AppForSEII2526.API.DTOs.DeviceDTOs.DevideRentalDTOs
{
    public class DeviceRentalDTO
    {
        public DeviceRentalDTO() { }
        public DeviceRentalDTO(
            int id,
            int year,
            double priceForRent,
            string name,
            string model,
            string color,
            string brand
          
            )
        {
            this.Id = id;
            this.Year = year;
            this.priceForRent = priceForRent;
            this.Name = name;
            this.Model = model;
            this.Color = color;
            this.Brand = brand;
         
        }

        public int Id { get; set; }
        public int Year { get; set; }
     


        public double priceForRent { get; set; }
        public string Name { get; set; }
        public string Model { get; set; }
        public string Color { get; set; }
        public string Brand { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is DeviceRentalDTO device)
            {
                return Id == device.Id &&
                       Name == device.Name &&
                       Model == device.Model &&
                       Color == device.Color &&
                       Brand == device.Brand &&
                       priceForRent == device.priceForRent &&
                       Year == device.Year;
            }
            return false;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(
                this.Id,
                this.Year,
                this.priceForRent,
                this.Name
                );
        }
    }
}