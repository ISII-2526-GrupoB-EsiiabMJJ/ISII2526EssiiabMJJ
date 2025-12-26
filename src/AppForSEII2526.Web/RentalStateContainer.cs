using AppForSEII2526.Web.API;

namespace AppForSEII2526.Web
{
    public class RentalStateContainer{
        //Hemos creado una instancia de Alquiler para cuando se cree una instancia de RentalStateContainer
        public RentalForCreateDTO Rental { get; private set; } = new RentalForCreateDTO()
        {
            RentalItems = new List<RentalItemDTO>()
        };

        // Hemos computado el precio total (TotalPrice) de los dispositivos seleccionados para alquilarlos
        public decimal TotalPrice
        {
            get
            {
                int numberOfDays = (Rental.RentalDateTo - Rental.RentalDateFrom).Days;
                return Convert.ToDecimal(Rental.RentalItems.Sum(ri => ri.PriceForRent * numberOfDays));
            }
        }

        public event Action? OnChange;

        private void NotifyStateChanged() => OnChange?.Invoke();



        public void AddDeviceToRental(RentalForCreateDTO rental)
        {
            foreach (var rentalItem in rental.RentalItems)
            {
                if (!Rental.RentalItems.Any(ri => ri.DeviceID == rentalItem.DeviceID))
                {
                    // Añadimos el dispositivo si no está en la lista
                    Rental.RentalItems.Add(new RentalItemDTO()
                    {
                        DeviceID = rentalItem.DeviceID,
                        PriceForRent = rentalItem.PriceForRent,
                        Description = rentalItem.Description,
                        Model = rentalItem.Model
                    });
                }
            }

        }
        //para borrar dispositivos de la lista de dispositivos seleccionados
        public void RemoveRentalItemToRent(RentalItemDTO item)
        {
            Rental.RentalItems.Remove(item);

        }
        //Hemos eliminado todos los dispositivos de la lista
        public void ClearRentingCart()
        {
            Rental.RentalItems.Clear();

        }

        
        //Casi hemos terminado el proceso de alquiler, por lo que, creamos un nuevo Alquiler
        public void RentalProcessed()
        {
            //Tenemos que terminar el proceso de alquilar, por lo que creamos un nuevo objeto sin datos
            Rental = new RentalForCreateDTO()
            {
                RentalItems = new List<RentalItemDTO>()
            };
        }
    }
}
