using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ECommerce.AvaloniaClient.Interfaces.Api;
using ECommerce.AvaloniaClient.Messages;
using ECommerce.Shared.Dtos.Addresses.Response;
using ECommerce.Shared.Dtos.Shared.Pagination;

namespace ECommerce.AvaloniaClient.ViewModels;

public partial class DeleteAddressViewModel : ObservableObject
{
    private readonly IAddressApiService _addressApiService;
    public ObservableCollection<AddressResponse> Addresses { get; } = [];
    [ObservableProperty] private AddressResponse? _selectedAddress;
    [ObservableProperty] private AddressResponse? _deletedAddress;
    [ObservableProperty] private string? _errorMessage;
    [ObservableProperty] private int _currentPage = 1;
    [ObservableProperty] private int _totalPages;
    [ObservableProperty] private int _pageSize = 10;
    [ObservableProperty] private int _totalCount;
    public bool IsInConfirmationMode => DeletedAddress is not null;

    public bool CanGoToPreviousPage => CurrentPage > 1;
    public bool CanGoToNextPage => CurrentPage < TotalPages;

    public DeleteAddressViewModel(IAddressApiService addressApiService)
    {
        _addressApiService = addressApiService;
        InitializeCommand.ExecuteAsync(null);
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        var queryParams = new AddressQueryParams
        {
            PageNumber = CurrentPage,
            PageSize = PageSize
        };
        var pagedResult = await _addressApiService.GetAllAddressesAsync(queryParams);

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
        
        OnPropertyChanged(nameof(CanGoToPreviousPage));
        OnPropertyChanged(nameof(CanGoToNextPage));
    }

    [RelayCommand]
    private async Task DeleteAddress()
    {
        ErrorMessage = null;

        var id = SelectedAddress!.Id;
        DeletedAddress = await _addressApiService.DeleteAddressAsync(id);

        if (DeletedAddress is not null)
        {
            ErrorMessage = null;
            SelectedAddress = null;
            WeakReferenceMessenger
                .Default
                .Send(new AddressRefreshMessage());
        }
        else
        {
            ErrorMessage = "Unable to delete address";
        }
        
        
    }

    [RelayCommand]
    private async Task GoToNextPage()
    {
        if (CanGoToNextPage)
        {
            CurrentPage++;
            await InitializeAsync();
        }
    }

    [RelayCommand]
    private async Task GoToPreviousPage()
    {
        if (CanGoToPreviousPage)
        {
            CurrentPage--;
            await InitializeAsync();
        }
    }

    [RelayCommand]
    private async Task DeleteAnotherAddress()
    {
        DeletedAddress = null;
        await InitializeAsync();
    }
}