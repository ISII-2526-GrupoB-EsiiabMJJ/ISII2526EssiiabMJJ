using System.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using AppForSEII2526.API.DTOs;
using AppForSEII2526.API.DTOs.ReviewDTOs;

namespace AppForSEII2526.API.DTOs.ReviewDTOs
                
{
    public class ReviewDetailsDTO
    {
        public ReviewDetailsDTO() { }

        public ReviewDetailsDTO(
            int id,
            string customerName,
            string customerCountry,
            string reviewTitle,
            DateTime dateOfReview,
            IList<ReviewDeviceDTO> reviewDetailsDevicesList
        )
        {
            this.Id = id;
            this.CustomerCountry = customerCountry;
            this.CustomerName = customerName;
            this.ReviewTitle = reviewTitle;
            this.DateOfReview = dateOfReview;
            this.ReviewDetailsDevicesList = reviewDetailsDevicesList;
        }

        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCountry { get; set; }
        public string ReviewTitle { get; set; }
        public DateTime DateOfReview { get; set; }

        public IList<ReviewDeviceDTO> ReviewDetailsDevicesList { get; set; }

        private bool TimeComparator(DateTime date1, DateTime date2, double milliseconds_tolerance = 500)
        {
            return Math.Abs((date1 - date2).TotalMilliseconds) < milliseconds_tolerance;
        }

        public override bool Equals(object? obj)
        {
            if (obj is ReviewDetailsDTO reviewDetailsDTO)
            {
                return
                    //CustomerName.Equals(reviewDetailsDTO.CustomerName) &&
                    Id.Equals(reviewDetailsDTO.Id) &&
                    CustomerCountry.Equals(reviewDetailsDTO.CustomerCountry) &&
                    ReviewTitle.Equals(reviewDetailsDTO.ReviewTitle) &&
                    ReviewDetailsDevicesList.SequenceEqual(reviewDetailsDTO.ReviewDetailsDevicesList) &&
                    TimeComparator(DateOfReview, reviewDetailsDTO.DateOfReview);

            }
            return false;
        }
    }

}