using CommunityToolkit.Mvvm.ComponentModel;

namespace ECommerce.AvaloniaClient.ViewModels.Helpers;

public partial class SaleItemViewModel : ObservableObject
{
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }

    [ObservableProperty] private int _quantity;

    public decimal Subtotal => UnitPrice * Quantity;
}