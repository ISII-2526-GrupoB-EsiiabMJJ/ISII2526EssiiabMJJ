namespace AppForSEII2526.API.DTOs.RentalDTOSs
{
    
    public class RentalItemDTO
    {
        public RentalItemDTO(int deviceID, string model, double priceForRent)
        {
            DeviceID = deviceID;
            PriceForRent = priceForRent;
            Model = model;
            
        }

        public int DeviceID { get; set; }


       


        public double PriceForRent { get; set; }

        public string Description { get; set; }

        public string Model { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is RentalItemDTO dTO &&
                   DeviceID == dTO.DeviceID &&
                   PriceForRent == dTO.PriceForRent &&
                   Model == dTO.Model;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(DeviceID, PriceForRent, Model);
        }
    }
}
