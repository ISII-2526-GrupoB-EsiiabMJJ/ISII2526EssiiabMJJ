using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using AppForSEII2526.API.DTOs.ReviewDTOs;

namespace AppForSEII2526.API.DTOs.ReviewDTOs
{
    public class CreateReviewDTO
    {
        public CreateReviewDTO()
        {
            ReviewItems = new List<ReviewItemDTO>();
        }

        public CreateReviewDTO(
            string customerId,

            string reviewTitle,
            string customerCountry,
            string customerName,
            DateTime dateOfReview,
            IList<ReviewItemDTO> reviewItems
            )
        {
            CustomerId = customerId;
            ReviewTitle = reviewTitle;
            CustomerName = customerName;
            CustomerCountry = customerCountry;

            DateOfReview = dateOfReview;

            ReviewItems = reviewItems;

        }

        [Required]
        public string CustomerId { get; set; }
        
        [StringLength(100, MinimumLength = 10, ErrorMessage = "Debe tener entre 10 y 100 caracteres el titulo de la reseña")]
        public string ReviewTitle { get; set; }
        
        [StringLength(40, MinimumLength = 4, ErrorMessage = "Debe tener entre 4 y 40 caracteres el nombre del país")]
        public string CustomerCountry { get; set; }

        [StringLength(20, MinimumLength = 3, ErrorMessage = "Debe tener entre 3 y 20 caracteres el nombre del cliente")]
        public string? CustomerName { get; set; }

        public DateTime DateOfReview { get; set; }


        [ValidateComplexType]
        public IList<ReviewItemDTO> ReviewItems { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj is CreateReviewDTO createDTO)
            {
                return CustomerId.Equals(createDTO.CustomerId) &&
                       ReviewTitle.Equals(createDTO.ReviewTitle) &&
                       CustomerCountry.Equals(createDTO.CustomerCountry) &&
                       //CustomerName.Equals(createDTO.CustomerName) &&
                       ReviewItems.SequenceEqual(createDTO.ReviewItems);
            }
            return false;
        }
    }
}
/*
{
     "customerId": 1,
     "reviewTitle": "Potencia brutal",
     "customerCountry": "España",
     "customerName": "Carlos",
     "dateOfReview": "2025-09-10T10:00:00",
     "reviewItems": [
       {
         "deviceId": 1,
         "rating": 5,
         "comment": "Máximo rendimiento en gaming e IA",
         "Name": "RTX 5090 Founders Edition",
         "Model": "NVIDIA GeForce RTX 5090"
       },
       {
         "deviceId": 2,
         "rating": 4,
         "comment": "Buen rendimiento pero algo ruidosa",
         "Name": "RTX 5080 Gaming Pro",
         "Model": "NVIDIA GeForce RTX 5080"
       }
     ]
   }
 */
