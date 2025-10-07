using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ECommerce.Shared.Dtos.Addresses.Request;
using ECommerce.AvaloniaClient.Interfaces.Api;
using ECommerce.AvaloniaClient.Messages;
using ECommerce.Shared.Dtos.Addresses.Response;
using ECommerce.Shared.Enums;

namespace ECommerce.AvaloniaClient.ViewModels;

public partial class AddAddressViewModel : ObservableValidator
{
    private readonly IAddressApiService _addressApiService;
    public IEnumerable<AddressType> AddressTypes => Enum.GetValues<AddressType>();

    public AddAddressViewModel(IAddressApiService addressApiService)
    {
        _addressApiService = addressApiService;
    }

    [ObservableProperty]
    [Required(ErrorMessage = "Street number is required")]
    [NotifyPropertyChangedFor(nameof(StreetNumberErrors))]
    private string _streetNumber = string.Empty;
    public string? StreetNumberErrors => GetErrors(nameof(StreetNumber))
        .FirstOrDefault()?
        .ErrorMessage;

    [ObservableProperty]
    [Required(ErrorMessage = "Street name is required")]
    [NotifyPropertyChangedFor(nameof(StreetNameErrors))]
    private string _streetName = string.Empty;
    public string? StreetNameErrors => GetErrors(nameof(StreetName))
        .FirstOrDefault()?
        .ErrorMessage;

    [ObservableProperty] 
    [Required(ErrorMessage = "City is required")] 
    [NotifyPropertyChangedFor(nameof(CityErrors))]
    private string _city = string.Empty;

    public string? CityErrors => GetErrors(nameof(City))
        .FirstOrDefault()?
        .ErrorMessage;

    [ObservableProperty] 
    [Required(ErrorMessage = "State is required")] 
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
    [NotifyPropertyChangedFor(nameof(CountryErrors))]
    private string _country = string.Empty;

    public string? CountryErrors => GetErrors(nameof(Country))
        .FirstOrDefault()?
        .ErrorMessage;

    [ObservableProperty] private string? _errorMessage;
    [ObservableProperty] private AddressType _selectedType;
    [ObservableProperty] private AddressResponse? _addedAddress;
    public bool IsInConfirmationMode => AddedAddress is not null;

    [RelayCommand]
    private async Task AddAddress()
    {
        ErrorMessage = null;
        ClearErrors();
        ValidateAllProperties();

        if (HasErrors)
        {
            return;
        }

        var createAddressRequest = new CreateAddressRequest
        {
            StreetNumber = StreetNumber,
            StreetName = StreetName,
            City = City,
            State = State,
            ZipCode = ZipCode,
            Country = Country,
            AddressType = SelectedType
        };

        AddedAddress = await _addressApiService.AddAddressAsync(createAddressRequest);

        if (AddedAddress is not null)
        {
            ErrorMessage = null;
            WeakReferenceMessenger
                .Default
                .Send(new AddressAddedMessage());
        }
        else
        {
            ErrorMessage = "Unable to add address";
        }
    }

    [RelayCommand]
    private void AddAnotherAddress()
    {
        AddedAddress = null;
        StreetNumber = string.Empty;
        StreetName = string.Empty;
        City = string.Empty;
        State = string.Empty;
        ZipCode = string.Empty;
        Country = string.Empty;
        SelectedType = AddressType.Home;
    }
}