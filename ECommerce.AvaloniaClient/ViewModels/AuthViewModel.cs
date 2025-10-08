using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using ECommerce.Shared.Dtos.Auth.Request;
using ECommerce.AvaloniaClient.Interfaces.Api;
using ECommerce.Shared.Enums;

namespace ECommerce.AvaloniaClient.ViewModels;

public partial class AuthViewModel : ObservableValidator
{
    private readonly ILoginApiService _loginApiService;
    public event Action<bool>? LoginSuccessful;
    public IEnumerable<AddressType> AddressTypes => Enum.GetValues<AddressType>(); 

    public AuthViewModel(ILoginApiService loginApiService)
    {
        _loginApiService = loginApiService;
    }

    [ObservableProperty]
    [Required(ErrorMessage = "First name is required")]
    [NotifyPropertyChangedFor(nameof(FirstNameErrors))]
    private string _firstName = string.Empty;

    public string? FirstNameErrors => GetErrors(nameof(FirstName))
        .FirstOrDefault()?
        .ErrorMessage;

    [ObservableProperty]
    [Required(ErrorMessage = "Last name is required")]
    [NotifyPropertyChangedFor(nameof(LastNameErrors))]
    private string _lastName = string.Empty;

    public string? LastNameErrors => GetErrors(nameof(LastName))
        .FirstOrDefault()?
        .ErrorMessage;

    [ObservableProperty]
    [Required(ErrorMessage = "Date of birth is required")]
    [NotifyPropertyChangedFor(nameof(SelectedBirthDateErrors))]
    private DateTimeOffset? _selectedBirthDate;

    public string? SelectedBirthDateErrors => GetErrors(nameof(SelectedBirthDate))
        .FirstOrDefault()?
        .ErrorMessage;

    public DateOnly? BirthDate => SelectedBirthDate.HasValue
        ? DateOnly.FromDateTime(SelectedBirthDate.Value.Date)
        : null;

    [ObservableProperty]
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    [NotifyPropertyChangedFor(nameof(EmailAddressErrors))]
    private string _emailAddress = string.Empty;

    public string? EmailAddressErrors => GetErrors(nameof(EmailAddress))
        .FirstOrDefault()?
        .ErrorMessage;

    [ObservableProperty]
    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 8 characters long")]
    [NotifyPropertyChangedFor(nameof(PasswordErrors))]
    private string _password = string.Empty;

    public string? PasswordErrors => GetErrors(nameof(Password))
        .FirstOrDefault()?
        .ErrorMessage;

    [ObservableProperty]
    [Required(ErrorMessage = "You must confirm your password")]
    [CustomValidation(typeof(AuthViewModel), nameof(ValidateConfirmPassword))]
    [NotifyPropertyChangedFor(nameof(ConfirmPasswordErrors))]
    private string _confirmPassword = string.Empty;

    public string? ConfirmPasswordErrors => GetErrors(nameof(ConfirmPassword))
        .FirstOrDefault()?
        .ErrorMessage;

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

    [ObservableProperty] 
    [Required(ErrorMessage = "Address type is required")]
    [NotifyPropertyChangedFor(nameof(SelectedTypeErrors))]
    private AddressType _selectedType;

    public string? SelectedTypeErrors => GetErrors(nameof(SelectedType))
        .FirstOrDefault()?
        .ErrorMessage;

    [ObservableProperty]
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    [NotifyPropertyChangedFor(nameof(LoginEmailErrors))]
    private string _loginEmail = string.Empty;

    public string? LoginEmailErrors => GetErrors(nameof(LoginEmail))
        .FirstOrDefault()?
        .ErrorMessage;

    [ObservableProperty]
    [Required(ErrorMessage = "Password is required")]
    [NotifyPropertyChangedFor(nameof(LoginPasswordErrors))]
    private string _loginPassword = string.Empty;

    public string? LoginPasswordErrors => GetErrors(nameof(LoginPassword))
        .FirstOrDefault()?
        .ErrorMessage;

    [ObservableProperty] private string? _successMessage;
    [ObservableProperty] private string? _errorMessage;

    [RelayCommand]
    private async Task Register()
    {
        SuccessMessage = null;
        ErrorMessage = null;

        ClearErrors();
        
        ValidateProperty(FirstName, nameof(FirstName));
        ValidateProperty(LastName, nameof(LastName));
        ValidateProperty(BirthDate, nameof(BirthDate));
        ValidateProperty(EmailAddress, nameof(EmailAddress));
        ValidateProperty(Password, nameof(Password));
        ValidateProperty(ConfirmPassword, nameof(ConfirmPassword));
        ValidateProperty(StreetNumber, nameof(StreetNumber));
        ValidateProperty(StreetName, nameof(StreetName));
        ValidateProperty(City, nameof(City));
        ValidateProperty(State, nameof(State));
        ValidateProperty(ZipCode, nameof(ZipCode));
        ValidateProperty(Country, nameof(Country));
        ValidateProperty(SelectedType, nameof(SelectedType));

        if (HasErrors)
        {
            return;
        }

        var registerRequest = new RegisterRequest
        {
            FirstName = FirstName,
            LastName = LastName,
            BirthDate = BirthDate!.Value,
            Email = EmailAddress,
            Password = Password,
            ConfirmPassword = ConfirmPassword,
            StreetNumber = StreetNumber,
            StreetName = StreetName,
            City = City,
            State = State,
            ZipCode = ZipCode,
            Country = Country,
            AddressType = SelectedType,
        };

        var success = await _loginApiService.RegisterAsync(registerRequest);

        if (success)
        {
            ErrorMessage = null;
            SuccessMessage = "Registration successful!\nYou may now login!";
            FirstName = string.Empty;
            LastName = string.Empty;
            EmailAddress = string.Empty;
            Password = string.Empty;
            ConfirmPassword = string.Empty;
            StreetNumber = string.Empty;
            StreetName = string.Empty;
            City = string.Empty;
            State = string.Empty;
            ZipCode = string.Empty;
            Country = string.Empty;
            SelectedType = AddressType.Home;
            SelectedBirthDate = null;
        }
        else
        {
            ErrorMessage = "Registration failed";
        }
    }

    [RelayCommand]
    private async Task Login()
    {
        SuccessMessage = null;
        ErrorMessage = null;

        ClearErrors();
        ValidateProperty(LoginEmail, nameof(LoginEmail));
        ValidateProperty(LoginPassword, nameof(LoginPassword));

        if (HasErrors)
        {
            return;
        }

        var loginRequest = new LoginRequest
        {
            Email = LoginEmail,
            Password = LoginPassword
        };
        
        var success = await _loginApiService.LoginAsync(loginRequest);

        if (success)
        {
            ErrorMessage = null;
            var principal = _loginApiService.GetPrincipalFromToken(_loginApiService.JwtToken!);
            var isAdmin = principal?.HasClaim(ClaimTypes.Role, "Admin") ?? false;

            LoginSuccessful?.Invoke(isAdmin);
        }
        else
        {
            ErrorMessage = "Login failed. Please check your credentials";
        }
    }

    [RelayCommand]
    private void Exit()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();
        }
    }

    public static ValidationResult? ValidateConfirmPassword(string confirmPassword, ValidationContext context)
    {
        var viewModel = (AuthViewModel)context.ObjectInstance;
        var password = viewModel.Password;

        if (confirmPassword != password)
        {
            return new ValidationResult("Passwords do not match");
        }

        return ValidationResult.Success;
    }
}