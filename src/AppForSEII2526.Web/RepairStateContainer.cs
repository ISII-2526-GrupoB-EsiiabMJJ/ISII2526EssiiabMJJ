using AppForSEII2526.Web.API;

namespace AppForSEII2526.Web
{
    public class RepairStateContainer
    {
        public IList<ReceiptItemForCreateDTO> ReceiptItems { get; private set; } =
            new List<ReceiptItemForCreateDTO>();

        public IList<RepairForReceiptDTO> SelectedRepairs { get; private set; } =
            new List<RepairForReceiptDTO>();

        public string? CustomerUserName { get; set; }

        public string? CustomerNameSurname { get; set; }

        public string? DeliveryAddress { get; set; }

        public PaymentMethodTypes PaymentMethod { get; set; } = PaymentMethodTypes.CreditCard;

        public event Action? OnChange;

        public bool HasItems => ReceiptItems.Any();

        public double TotalPrice => SelectedRepairs.Sum(repair => repair.Cost);

        public bool ContainsRepair(int repairId)
        {
            return ReceiptItems.Any(item => item.RepairId == repairId);
        }

        public void AddRepair(RepairForReceiptDTO repair)
        {
            if (ContainsRepair(repair.Id))
            {
                return;
            }

            SelectedRepairs.Add(repair);

            ReceiptItems.Add(new ReceiptItemForCreateDTO
            {
                RepairId = repair.Id,
                Model = string.Empty
            });

            NotifyStateChanged();
        }

        public void RemoveRepair(int repairId)
        {
            var receiptItem = ReceiptItems.FirstOrDefault(item => item.RepairId == repairId);

            if (receiptItem is not null)
            {
                ReceiptItems.Remove(receiptItem);
            }

            var selectedRepair = SelectedRepairs.FirstOrDefault(repair => repair.Id == repairId);

            if (selectedRepair is not null)
            {
                SelectedRepairs.Remove(selectedRepair);
            }

            NotifyStateChanged();
        }

        public void UpdateModel(int repairId, string? model)
        {
            var receiptItem = ReceiptItems.FirstOrDefault(item => item.RepairId == repairId);

            if (receiptItem is not null)
            {
                receiptItem.Model = model ?? string.Empty;
                NotifyStateChanged();
            }
        }

        public ReceiptForCreateDTO ToReceiptForCreateDTO()
        {
            return new ReceiptForCreateDTO
            {
                CustomerUserName = CustomerUserName ?? string.Empty,
                CustomerNameSurname = CustomerNameSurname ?? string.Empty,
                DeliveryAddress = DeliveryAddress ?? string.Empty,
                PaymentMethod = PaymentMethod,
                ReceiptItems = ReceiptItems.ToList()
            };
        }

        public void Clear()
        {
            ReceiptItems.Clear();
            SelectedRepairs.Clear();
            CustomerUserName = null;
            CustomerNameSurname = null;
            DeliveryAddress = null;
            PaymentMethod = PaymentMethodTypes.CreditCard;

            NotifyStateChanged();
        }

        private void NotifyStateChanged()
        {
            OnChange?.Invoke();
        }
    }
}