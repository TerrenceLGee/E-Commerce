using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ECommerce.Shared.Dtos.Products.Request;
using ECommerce.AvaloniaClient.Interfaces.Api;
using ECommerce.AvaloniaClient.Messages;
using ECommerce.Shared.Dtos.Categories.Response;
using ECommerce.Shared.Dtos.Products.Response;
using ECommerce.Shared.Dtos.Shared.Pagination;
using ECommerce.Shared.Enums;

namespace ECommerce.AvaloniaClient.ViewModels;

public partial class AddProductViewModel : ObservableValidator
{
    private readonly IProductsApiService _productsApiService;
    private readonly ICategoriesApiService _categoriesApiService;

    public AddProductViewModel(
        IProductsApiService productsApiService,
        ICategoriesApiService categoriesApiService)
    {
        _productsApiService = productsApiService;
        _categoriesApiService = categoriesApiService;
        InitializeCommand.ExecuteAsync(null);
    }

    [ObservableProperty] private bool _isLoading;
    
    [ObservableProperty]
    [Required(ErrorMessage = "Product name is required")]
    [MaxLength(200, ErrorMessage = "Product name can not be greater than 200 characters")]
    [NotifyPropertyChangedFor(nameof(ProductNameErrors))]
    private string _productName = string.Empty;

    public string? ProductNameErrors => GetErrors(nameof(ProductName))
        .FirstOrDefault()?
        .ErrorMessage;

    [ObservableProperty]
    [MaxLength(1000, ErrorMessage = "Description can not be greater than 1000 characters")]
    [NotifyPropertyChangedFor(nameof(DescriptionErrors))]
    private string? _description;

    public string? DescriptionErrors => GetErrors(nameof(Description))
        .FirstOrDefault()?
        .ErrorMessage;

    [ObservableProperty]
    [MaxLength(50, ErrorMessage = "Stock keeping unit must not be greater than 50 characters")]
    [NotifyPropertyChangedFor(nameof(StockKeepingUnitErrors))]
    private string? _stockKeepingUnit;

    public string? StockKeepingUnitErrors => GetErrors(nameof(StockKeepingUnit))
        .FirstOrDefault()?
        .ErrorMessage;

    [ObservableProperty]
    [Required(ErrorMessage = "Product must have a price")]
    [Range(0.01, 1000000, ErrorMessage = "Price must be greater than $0.00 and less than $1000000")]
    [NotifyPropertyChangedFor(nameof(PriceErrors))]
    private decimal _price;

    public string? PriceErrors => GetErrors(nameof(Price))
        .FirstOrDefault()?
        .ErrorMessage;

    [ObservableProperty]
    [Required(ErrorMessage = "Discount must be chosen")]
    [NotifyPropertyChangedFor(nameof(DiscountErrors))]
    private DiscountStatus _selectedDiscount;

    public string? DiscountErrors => GetErrors(nameof(SelectedDiscount))
        .FirstOrDefault()?
        .ErrorMessage;

    [ObservableProperty]
    [Required(ErrorMessage = "Must enter how much of this Product is in stock")]
    [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be a non-negative integer")]
    [NotifyPropertyChangedFor(nameof(StockQuantityErrors))]
    private int _stockQuantity;

    public string? StockQuantityErrors => GetErrors(nameof(StockQuantity))
        .FirstOrDefault()?
        .ErrorMessage;

    [ObservableProperty]
    [Required(ErrorMessage = "Product must belong to a category")]
    [NotifyPropertyChangedFor(nameof(CategoryErrors))]
    private CategoryResponse? _selectedCategory;

    public string? CategoryErrors => GetErrors(nameof(SelectedCategory))
        .FirstOrDefault()?
        .ErrorMessage;

    [ObservableProperty] private bool _isActive;

    [ObservableProperty] private string? _errorMessage;
    [ObservableProperty] private ProductResponse? _addedProduct;
    public bool IsInConfirmationMode => AddedProduct is not null;

    public ObservableCollection<CategoryResponse> AvailableCategories { get; } = [];
    public IEnumerable<DiscountStatus> AvailableDiscounts => Enum.GetValues<DiscountStatus>();

    [RelayCommand]
    private async Task InitializeAsync()
    {
        var categories = await _categoriesApiService.GetCategoriesAsync(new CategoryQueryParams());

        if (categories is not null)
        {
            foreach (var category in categories.Items)
            {
                AvailableCategories.Add(category);
            }
        }
    }

    [RelayCommand]
    private async Task AddProduct()
    {
        ErrorMessage = null;

        ClearErrors();
        ValidateAllProperties();

        if (HasErrors)
        {
            return;
        }

        var createProductRequest = new CreateProductRequest
        {
            Name = ProductName,
            Description = Description,
            StockKeepingUnit = StockKeepingUnit,
            Price = Price,
            Discount = SelectedDiscount,
            StockQuantity = StockQuantity,
            CategoryId = SelectedCategory!.Id,
            IsActive = IsActive
        };

        AddedProduct = await _productsApiService.CreateProductAsync(createProductRequest);

        if (AddedProduct is not null)
        {
            ErrorMessage = null;
            WeakReferenceMessenger
                .Default
                .Send(new ProductRefreshMessage());
        }
        else
        {
            ErrorMessage = "Unable to add product";
        }
    }

    [RelayCommand]
    private void AddAnotherProduct()
    {
        AddedProduct = null;
        ProductName = string.Empty;
        Description = string.Empty;
        StockKeepingUnit = string.Empty;
        Price = 0.0m;
        SelectedDiscount = DiscountStatus.None;
        StockQuantity = 0;
        SelectedCategory = null;
        IsActive = false;
    }
}