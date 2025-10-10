using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ECommerce.Shared.Dtos.Addresses.Response;
using ECommerce.AvaloniaClient.Interfaces.Api;
using System.Threading.Tasks;

namespace ECommerce.AvaloniaClient.ViewModels;

public partial class AddressDetailViewModel : ObservableObject
{
    private readonly IAddressApiService _addressApiService;
    public event Action? BackRequested;

    [ObservableProperty] private AddressResponse? _selectedAddress;

    public AddressDetailViewModel(IAddressApiService addressApiService)
    {
        _addressApiService = addressApiService;
    }

    [RelayCommand]
    private void GoBack()
    {
        BackRequested?.Invoke();
    }

    public async Task InitializeAsync(int addressId)
    {
        SelectedAddress = await _addressApiService.GetAddressByIdAsync(addressId);
    }
}