using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ECommerce.Shared.Dtos.Addresses.Response;
using ECommerce.AvaloniaClient.Interfaces.Api;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ECommerce.Shared.Dtos.Shared.Pagination;
using ECommerce.AvaloniaClient.Messages;

namespace ECommerce.AvaloniaClient.ViewModels;

public partial class AddressesViewModel : ObservableObject, IRecipient<AddressAddedMessage>
{
    private readonly IAddressApiService _addressApiService;
    public ObservableCollection<AddressResponse> Addresses { get; } = [];
    public event Action<int>? AddressSelected;

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private AddressResponse? _selectedAddress;
    [ObservableProperty] private int _currentPage = 1;
    [ObservableProperty] private int _totalPages;
    [ObservableProperty] private int _pageSize = 10;
    [ObservableProperty] private int _totalCount;

    public bool CanGoToPreviousPage => CurrentPage > 1;
    public bool CanGoToNextPage => CurrentPage < _totalPages;

    public AddressesViewModel(IAddressApiService addressApiService)
    {
        _addressApiService = addressApiService;
        WeakReferenceMessenger.Default.Register(this);
        LoadAddressesCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadAddressesAsync()
    {
        IsLoading = true;

        var paginationParams = new PaginationParams { PageNumber = CurrentPage, PageSize = PageSize };
        var pagedResult = await _addressApiService.GetAllAddressesAsync(paginationParams);

        if (pagedResult is not null)
        {
            Addresses.Clear();
            foreach (var address in pagedResult.Items)
            {
                Addresses.Add(address);
            }

            TotalPages = pagedResult.TotalPages;
            TotalCount = pagedResult.TotalCount;
        }

        IsLoading = false;
        
        OnPropertyChanged(nameof(CanGoToPreviousPage));
        OnPropertyChanged(nameof(CanGoToNextPage));
    }

    [RelayCommand]
    private async Task GoToNextPage()
    {
        if (CanGoToNextPage)
        {
            CurrentPage++;
            await LoadAddressesAsync();
        }
    }

    [RelayCommand]
    private async Task GoToPreviousPage()
    {
        if (CanGoToPreviousPage)
        {
            CurrentPage--;
            await LoadAddressesAsync();
        }
    }

    partial void OnSelectedAddressChanged(AddressResponse? value)
    {
        if (value is not null)
        {
            AddressSelected?.Invoke(value.Id);
        }
    }

    public void Receive(AddressAddedMessage message)
    {
        LoadAddressesCommand.Execute(null);
    }
}