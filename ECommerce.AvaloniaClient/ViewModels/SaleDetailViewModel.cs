using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ECommerce.Shared.Dtos.Sales.Response;
using ECommerce.AvaloniaClient.Interfaces.Api;

namespace ECommerce.AvaloniaClient.ViewModels;

public partial class SaleDetailViewModel : ObservableObject
{
    private readonly ISalesApiService _salesApiService;
    public event Action? BackRequested;

    [ObservableProperty] private SaleResponse? _selectedSale;
    [ObservableProperty] private bool _isLoading;

    public SaleDetailViewModel(ISalesApiService salesApiService)
    {
        _salesApiService = salesApiService;
    }

    [RelayCommand]
    private void GoBack()
    {
        BackRequested?.Invoke();
    }

    public async Task InitializeAsync(int saleId)
    {
        IsLoading = true;
        SelectedSale = await _salesApiService.GetSaleByIdAsync(saleId);
        IsLoading = false;
    }
}