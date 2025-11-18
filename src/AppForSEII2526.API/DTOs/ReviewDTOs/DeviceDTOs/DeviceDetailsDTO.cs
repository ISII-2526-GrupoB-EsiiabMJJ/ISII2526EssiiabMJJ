using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AppForSEII2526.API.DTOs.ReviewDTOs
{
    public class DeviceDetailsDTO
    {
        public DeviceDetailsDTO() { }
        public DeviceDetailsDTO(
            int id,
            string name,
            string brand,
            string color,
            int year,
            string model
            )
        {
            Id = id;
            Name = name;
            Brand = brand;
            Color = color;
            Year = year;
            Model = model;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public string Color { get; set; }
        public int Year { get; set; }
        public string Model { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is DeviceDetailsDTO deviceDto)
            {
                return
                    Id.Equals(deviceDto.Id) &&
                    Name.Equals(deviceDto.Name) &&
                    Brand.Equals(deviceDto.Brand) &&
                    Color.Equals(deviceDto.Color) &&
                    Year.Equals(deviceDto.Year) &&
                    Model.Equals(deviceDto.Model);
            }
            return false;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(
                Id,
                Name,
                Brand,
                Color,
                Year,
                Model
                );
        }
    }
}