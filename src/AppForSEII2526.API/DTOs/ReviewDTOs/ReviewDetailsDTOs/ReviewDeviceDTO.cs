using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AppForSEII2526.API.DTOs.ReviewDTOs
{
    public class ReviewDeviceDTO
    {
        public ReviewDeviceDTO() { }
        public ReviewDeviceDTO(
            int id,
            string name,
            string model,
            int year,
            int rating,
            string comment

        )
        {
            Id = id;
            Name = name;
            Model = model;
            Year = year;
            Rating = rating;
            Comment = comment;

        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is ReviewDeviceDTO deviceDto)
            {
                return
                    Id.Equals(deviceDto.Id) &&
                    Name.Equals(deviceDto.Name) &&
                    Model.Equals(deviceDto.Model) &&
                    Year.Equals(deviceDto.Year) &&
                    Rating.Equals(deviceDto.Rating) &&
                    Comment.Equals(deviceDto.Comment);
            }
            return false;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(
                Id,
                Name,
                Model,
                Year,
                Rating,
                Comment
            );
        }
    }
}