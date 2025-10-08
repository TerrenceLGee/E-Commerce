using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ECommerce.Shared.Dtos.Shared.Pagination;
using ECommerce.Shared.Enums;
using ECommerce.AvaloniaClient.Interfaces.Api;
using ECommerce.AvaloniaClient.Messages;
using ECommerce.AvaloniaClient.ViewModels.Helpers;
using ECommerce.Shared.Dtos.Addresses.Response;
using ECommerce.Shared.Dtos.Products.Response;
using ECommerce.Shared.Dtos.Sales.Response;
using ECommerce.Shared.Dtos.Sales.Request;

namespace ECommerce.AvaloniaClient.ViewModels;

public partial class CreateSaleViewModel : ObservableValidator
{
    private readonly ISalesApiService _salesApiService;
    private readonly IProductsApiService _productsApiService;
    private readonly IAddressApiService _addressApiService;

    public CreateSaleViewModel(
        ISalesApiService salesApiService,
        IProductsApiService productsApiService,
        IAddressApiService addressApiService)
    {
        _salesApiService = salesApiService;
        _productsApiService = productsApiService;
        _addressApiService = addressApiService;
        InitializeCommand.ExecuteAsync(null);
    }
    
    public ObservableCollection<ProductResponse> AvailableProducts { get; } = [];
    public ObservableCollection<AddressResponse> SavedAddresses { get; } = [];
    public ObservableCollection<SaleItemViewModel> ShoppingCart { get; } = [];
    public IEnumerable<AddressType> AddressTypes => Enum.GetValues<AddressType>();

    [ObservableProperty] private ProductResponse? _selectedProduct;
    [ObservableProperty] private AddressResponse? _selectedSavedAddress;
    [ObservableProperty] private SaleResponse? _addedSale;
    [ObservableProperty] private int _quantityToAdd = 1;
    [ObservableProperty] private string? _errorMessage;
    [ObservableProperty] private AddressType _selectedAddressType;

    [ObservableProperty]
    [Required(ErrorMessage = "Street number is required")]
    [MaxLength(15, ErrorMessage = "Street number must be no more than 15 characters")]
    [NotifyPropertyChangedFor(nameof(StreetNumberErrors))]
    private string _streetNumber = string.Empty;

    public string? StreetNumberErrors => GetErrors(nameof(StreetNumber))
        .FirstOrDefault()?
        .ErrorMessage;

    [ObservableProperty]
    [Required(ErrorMessage = "Street name is required")]
    [MaxLength(30, ErrorMessage = "Street name must be no more than 30 characters")]
    [NotifyPropertyChangedFor(nameof(StreetNameErrors))]
    private string _streetName = string.Empty;

    public string? StreetNameErrors => GetErrors(nameof(StreetName))
        .FirstOrDefault()?
        .ErrorMessage;

    [ObservableProperty] [Required(ErrorMessage = "City is required")] 
    [MaxLength(20, ErrorMessage = "City name can be no longer than 20 characters")]
    [NotifyPropertyChangedFor(nameof(CityErrors))]
    private string _city = string.Empty;

    public string? CityErrors => GetErrors(nameof(City))
        .FirstOrDefault()?
        .ErrorMessage;

    [ObservableProperty] [Required(ErrorMessage = "State is required")] 
    [MaxLength(20, ErrorMessage = "State name can be no longer than 20 characters")]
    [NotifyPropertyChangedFor(nameof(StateErrors))]
    private string _state = string.Empty;

    public string? StateErrors => GetErrors(nameof(State))
        .FirstOrDefault()?
        .ErrorMessage;

    [ObservableProperty]
    [Required(ErrorMessage = "Zip code is required")]
    [NotifyPropertyChangedFor(nameof(ZipCodeErrors))]
    private string _zipCode = string.Empty;

    public string? ZipCodeErrors => GetErrors(nameof(ZipCode))
        .FirstOrDefault()?
        .ErrorMessage;

    [ObservableProperty]
    [Required(ErrorMessage = "Country is required")]
    [MaxLength(25, ErrorMessage = "Country name can be no longer than 25 characters")]
    [NotifyPropertyChangedFor(nameof(CountryErrors))]
    private string _country = string.Empty;

    public string? CountryErrors => GetErrors(nameof(Country))
        .FirstOrDefault()?
        .ErrorMessage;

    [ObservableProperty]
    [Required(ErrorMessage = "Address type is required")]
    [NotifyPropertyChangedFor(nameof(AddressTypeErrors))]
    private AddressType _addressType;

    public string? AddressTypeErrors => GetErrors(nameof(AddressType))
        .FirstOrDefault()?
        .ErrorMessage;

    [ObservableProperty]
    [MaxLength(2000, ErrorMessage = "Sale notes can be no longer than 2000 characters")]
    [NotifyPropertyChangedFor(nameof(NotesErrors))]
    private string? _notes;

    public string? NotesErrors => GetErrors(nameof(Notes))
        .FirstOrDefault()?
        .ErrorMessage;

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private int _currentAddressPage = 1;
    [ObservableProperty] private int _currentProductPage = 1;
    [ObservableProperty] private int _totalAddressPages;
    [ObservableProperty] private int _totalProductPages;
    [ObservableProperty] private int _addressPageSize = 10;
    [ObservableProperty] private int _productPageSize = 10;
    [ObservableProperty] private int _totalAddressCount;
    [ObservableProperty] private int _totalProductCount;
    public bool CanGoToPreviousAddressPage => CurrentAddressPage > 1;
    public bool CanGoToNextAddressPage => CurrentAddressPage < TotalAddressPages;
    public bool CanGoToPreviousProductPage => CurrentProductPage > 1;
    public bool CanGoToNextProductPage => CurrentProductPage < TotalProductPages;
    public bool IsInConfirmationMode => AddedSale is not null;

    [RelayCommand]
    public async Task InitializeAsync()
    {
        await LoadAddressesAsync();
        await LoadProductsAsync();
    }

    [RelayCommand]
    private async Task PlaceOrder()
    {
        var createSaleRequest = new CreateSaleRequest
        {
            StreetNumber = StreetNumber,
            StreetName = StreetName,
            City = City,
            State = State,
            ZipCode = ZipCode,
            Country = Country,
            AddressType = SelectedAddressType,
            Notes = Notes,
            Items = ShoppingCart.Select(item => new SaleItemRequest
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
            }).ToList()
        };

        AddedSale = await _salesApiService.CreateSaleAsync(createSaleRequest);

        if (AddedSale is not null)
        {
            ErrorMessage = null;
            WeakReferenceMessenger
                .Default
                .Send(new SaleAddedMessage());
        }
        else
        {
            ErrorMessage = "Unable to create sale";
        }
    }

    [RelayCommand]
    private async Task LoadAddressesAsync()
    {
        IsLoading = true;

        var queryParams = new AddressQueryParams { PageNumber = CurrentAddressPage, PageSize = AddressPageSize };

        var pagedResult = await _addressApiService.GetAllAddressesAsync(queryParams);

        if (pagedResult is not null)
        {
            SavedAddresses.Clear();
            foreach (var address in pagedResult.Items)
            {
                SavedAddresses.Add(address);
            }

            TotalAddressPages = pagedResult.TotalPages;
            TotalAddressCount = pagedResult.TotalCount;
        }
        IsLoading = false;
        
        OnPropertyChanged(nameof(CanGoToPreviousAddressPage));
        OnPropertyChanged(nameof(CanGoToNextAddressPage));
    }

    [RelayCommand]
    private async Task GoToNextAddressPage()
    {
        if (CanGoToNextAddressPage)
        {
            CurrentAddressPage++;
            await LoadAddressesAsync();
        }
    }

    [RelayCommand]
    private async Task GoToPreviousAddressPage()
    {
        if (CanGoToPreviousAddressPage)
        {
            CurrentAddressPage--;
            await LoadAddressesAsync();
        }
    }

    [RelayCommand]
    private async Task LoadProductsAsync()
    {
        IsLoading = true;

        var queryParams = new ProductQueryParams { PageNumber = CurrentProductPage, PageSize = ProductPageSize};

        var pagedResult = await _productsApiService.GetProductsAsync(queryParams);

        if (pagedResult is not null)
        {
            AvailableProducts.Clear();
            foreach (var product in pagedResult.Items)
            {
                AvailableProducts.Add(product);
            }

            TotalProductPages = pagedResult.TotalPages;
            TotalProductCount = pagedResult.TotalCount;
        }
        IsLoading = false;
        
        OnPropertyChanged(nameof(CanGoToPreviousProductPage));
        OnPropertyChanged(nameof(CanGoToNextProductPage));
    }

    [RelayCommand]
    private async Task GoToNextProductPage()
    {
        if (CanGoToNextProductPage)
        {
            CurrentProductPage++;
            await LoadProductsAsync();
        }
    }

    [RelayCommand]
    private async Task GoToPreviousProductPage()
    {
        if (CanGoToPreviousProductPage)
        {
            CurrentProductPage--;
            await LoadProductsAsync();
        }
    }

    [RelayCommand]
    private void AddToCart()
    {
        if (SelectedProduct is null || QuantityToAdd <= 0)
        {
            return;
        }

        var cartItem = new SaleItemViewModel
        {
            ProductId = SelectedProduct.Id,
            Name = SelectedProduct.Name,
            Quantity = QuantityToAdd,
            UnitPrice = SelectedProduct.Price
        };

        ShoppingCart.Add(cartItem);
    }

    [RelayCommand]
    private void CreateAnotherSale()
    {
        AddedSale = null;
        StreetNumber = string.Empty;
        StreetName = string.Empty;
        City = string.Empty;
        State = string.Empty;
        ZipCode = string.Empty;
        Country = string.Empty;
        Notes = string.Empty;
        SelectedProduct = null;
        SelectedSavedAddress = null;
        QuantityToAdd = 1;
    }

    partial void OnSelectedSavedAddressChanged(AddressResponse? value)
    {
        if (value is not null)
        {
            StreetNumber = value.StreetNumber;
            StreetName = value.StreetName;
            City = value.City;
            State = value.State;
            Country = value.Country;
            ZipCode = value.ZipCode;
        }
    }
}