using System;
using System.Collections.Generic;
using System.Linq;
using AppForSEII2526.Web.API;

namespace AppForSEII2526.Web;

public class PurchaseStateContainer
{
    public IList<PurchaseItemDTO> PurchaseItems { get; private set; } = new List<PurchaseItemDTO>();

    public string? CustomerUserName { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? DeliveryAddress { get; set; }
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.CreditCard;

    public event Action? OnChange;

    public bool HasItems => PurchaseItems.Any();

    public double TotalPrice => PurchaseItems.Sum(item => item.Price * item.Quantity);

    public int TotalQuantity => PurchaseItems.Sum(item => item.Quantity);

    public bool ContainsDevice(int deviceId)
    {
        return PurchaseItems.Any(item => item.DeviceId == deviceId);
    }

    public void AddDevice(DeviceForPurchaseDTO device)
    {
        var existingItem = PurchaseItems.FirstOrDefault(item => item.DeviceId == device.Id);

        if (existingItem is not null)
        {
            existingItem.Quantity++;
        }
        else
        {
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

        NotifyStateChanged();
    }

    public void RemoveDevice(int deviceId)
    {
        var item = PurchaseItems.FirstOrDefault(item => item.DeviceId == deviceId);

        if (item is not null)
        {
            PurchaseItems.Remove(item);
            NotifyStateChanged();
        }
    }

    public void UpdateQuantity(int deviceId, int quantity)
    {
        var item = PurchaseItems.FirstOrDefault(item => item.DeviceId == deviceId);

        if (item is not null)
        {
            item.Quantity = Math.Max(1, quantity);
            NotifyStateChanged();
        }
    }

    public void UpdateDescription(int deviceId, string? description)
    {
        var item = PurchaseItems.FirstOrDefault(item => item.DeviceId == deviceId);

        if (item is not null)
        {
            item.Description = description;
            NotifyStateChanged();
        }
    }

    public PurchaseForCreateDTO ToPurchaseForCreateDTO()
    {
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

    public void Clear()
    {
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