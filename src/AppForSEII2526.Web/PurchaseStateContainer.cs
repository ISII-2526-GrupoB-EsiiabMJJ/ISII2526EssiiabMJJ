using System;
using System.Collections.Generic;
using System.Linq;
using AppForSEII2526.Web.API;

namespace AppForSEII2526.Web;

public class PurchaseStateContainer
{
    // Lista temporal de dispositivos seleccionados antes de confirmar la compra.
    public IList<PurchaseItemDTO> PurchaseItems { get; private set; } = new List<PurchaseItemDTO>();

    // Datos del cliente que se mantienen mientras el usuario completa el proceso de compra.
    public string? CustomerUserName { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? DeliveryAddress { get; set; }

    // Método de pago seleccionado. Por defecto se inicia con tarjeta.
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.CreditCard;

    // Evento utilizado para avisar a los componentes cuando cambia el estado del carrito o del formulario.
    public event Action? OnChange;

    // Indica si hay algún dispositivo seleccionado en el carrito.
    public bool HasItems => PurchaseItems.Any();

    // Precio total calculado a partir del precio y la cantidad de cada dispositivo.
    public double TotalPrice => PurchaseItems.Sum(item => item.Price * item.Quantity);

    // Cantidad total de unidades seleccionadas en la compra.
    public int TotalQuantity => PurchaseItems.Sum(item => item.Quantity);

    public bool ContainsDevice(int deviceId)
    {
        // Permite saber si un dispositivo ya está incluido en el carrito.
        return PurchaseItems.Any(item => item.DeviceId == deviceId);
    }

    public void AddDevice(DeviceForPurchaseDTO device)
    {
        // Se comprueba si el dispositivo ya estaba en el carrito.
        var existingItem = PurchaseItems.FirstOrDefault(item => item.DeviceId == device.Id);

        if (existingItem is not null)
        {
            // Si ya existe, se aumenta la cantidad en lugar de duplicar la línea.
            existingItem.Quantity++;
        }
        else
        {
            // Si no existe, se añade una nueva línea de compra con una unidad inicial.
            PurchaseItems.Add(new PurchaseItemDTO
            {
                DeviceId = device.Id,
                Brand = device.Brand,
                Model = device.Model,
                Color = device.Color,
                Price = device.Price,
                Quantity = 1,
                Description = string.Empty
            });
        }

        // Se notifica el cambio para que la interfaz actualice totales y carrito.
        NotifyStateChanged();
    }

    public void RemoveDevice(int deviceId)
    {
        // Se localiza el dispositivo que se quiere eliminar del carrito.
        var item = PurchaseItems.FirstOrDefault(item => item.DeviceId == deviceId);

        if (item is not null)
        {
            // Si existe, se elimina la línea completa de la compra.
            PurchaseItems.Remove(item);
            NotifyStateChanged();
        }
    }

    public void UpdateQuantity(int deviceId, int quantity)
    {
        // Se busca la línea correspondiente al dispositivo seleccionado.
        var item = PurchaseItems.FirstOrDefault(item => item.DeviceId == deviceId);

        if (item is not null)
        {
            // La cantidad mínima permitida es 1 para evitar líneas con cantidad cero o negativa.
            item.Quantity = Math.Max(1, quantity);
            NotifyStateChanged();
        }
    }

    public void UpdateDescription(int deviceId, string? description)
    {
        // Se actualiza la descripción asociada a un dispositivo concreto de la compra.
        var item = PurchaseItems.FirstOrDefault(item => item.DeviceId == deviceId);

        if (item is not null)
        {
            item.Description = description;
            NotifyStateChanged();
        }
    }

    public PurchaseForCreateDTO ToPurchaseForCreateDTO()
    {
        // Se construye el DTO que se enviará al backend al confirmar la compra.
        // Los valores nulos se sustituyen por cadena vacía para evitar enviar null al controlador.
        return new PurchaseForCreateDTO
        {
            CustomerUserName = CustomerUserName ?? string.Empty,
            Name = Name ?? string.Empty,
            Surname = Surname ?? string.Empty,
            DeliveryAddress = DeliveryAddress ?? string.Empty,
            PaymentMethod = PaymentMethod,
            PurchaseItems = PurchaseItems.ToList()
        };
    }

    public void ClearCart()
    {
        PurchaseItems.Clear();
        NotifyStateChanged();
    }

    public void Clear()
    {
        // Se limpia el estado tras finalizar la compra o reiniciar el proceso.
        PurchaseItems.Clear();
        CustomerUserName = null;
        Name = null;
        Surname = null;
        DeliveryAddress = null;
        PaymentMethod = PaymentMethod.CreditCard;

        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}