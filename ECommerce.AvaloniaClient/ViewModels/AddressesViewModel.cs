using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ECommerce.Shared.Dtos.Addresses.Response;
using ECommerce.AvaloniaClient.Interfaces.Api;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ECommerce.Shared.Dtos.Shared.Pagination;
using ECommerce.AvaloniaClient.Messages;
using ECommerce.Shared.Enums;

namespace ECommerce.AvaloniaClient.ViewModels;

public partial class AddressesViewModel : ObservableObject, IRecipient<AddressAddedMessage>
{
    private readonly IAddressApiService _addressApiService;
    public ObservableCollection<AddressResponse> Addresses { get; } = [];
    public event Action<int>? AddressSelected;

    public IEnumerable<OrderByOptions> SortOptions => new List<OrderByOptions>
    {
        OrderByOptions.DefaultOrder,
        OrderByOptions.IdAsc,
        OrderByOptions.IdDesc,
        OrderByOptions.CustomerIdDesc,
    };

    public IEnumerable<AddressType> AddressTypes => Enum.GetValues<AddressType>();
    
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private AddressResponse? _selectedAddress;
    [ObservableProperty] private int _currentPage = 1;
    [ObservableProperty] private int _totalPages;
    [ObservableProperty] private int _pageSize = 10;
    [ObservableProperty] private int _totalCount;
    [ObservableProperty] private string? _filterByStreetNumber;
    [ObservableProperty] private string? _filterByStreetName;
    [ObservableProperty] private string? _filterByCity;
    [ObservableProperty] private string? _filterByState;
    [ObservableProperty] private string? _filterByZipCode;
    [ObservableProperty] private string? _filterByCountry;
    [ObservableProperty] private AddressType? _filterByType;
    [ObservableProperty] private OrderByOptions _selectedOption;

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

        var queryParams = new AddressQueryParams
        {
            PageNumber = CurrentPage,
            PageSize = PageSize,
            OrderBy = SelectedOption,
            StreetNumber = FilterByStreetNumber,
            StreetName = FilterByStreetName,
            City = FilterByCity,
            State = FilterByState,
            ZipCode = FilterByZipCode,
            Country = FilterByCountry,
            AddressType = FilterByType
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