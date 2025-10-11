using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ECommerce.AvaloniaClient.Interfaces.Api;
using ECommerce.Shared.Dtos.Products.Response;
using ECommerce.Shared.Dtos.Shared.Pagination;
using ECommerce.AvaloniaClient.Messages;
using ECommerce.Shared.Dtos.Categories.Response;
using ECommerce.Shared.Enums;

namespace ECommerce.AvaloniaClient.ViewModels;

public partial class ProductsViewModel : ObservableRecipient, IRecipient<ProductRefreshMessage>
{
    private readonly IProductsApiService _productsApiService;
    private readonly ICategoriesApiService _categoriesApiService;
    public ObservableCollection<ProductResponse> Products { get; } = [];
    public ObservableCollection<CategoryResponse> Categories { get; } = [];
    public event Action<int>? ProductSelected;

    public IEnumerable<OrderByOptions> SortOptions => new List<OrderByOptions>
    {
        OrderByOptions.DefaultOrder,
        OrderByOptions.NameDesc,
        OrderByOptions.PriceAsc,
        OrderByOptions.PriceDesc,
        OrderByOptions.IdDesc,
    };

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private ProductResponse? _selectedProduct;
    [ObservableProperty] private CategoryResponse? _selectedCategory;
    [ObservableProperty] private int _currentPage = 1;
    [ObservableProperty] private int _totalPages;
    [ObservableProperty] private int _pageSize = 10;
    [ObservableProperty] private int _totalCount;
    [ObservableProperty] private string? _filterByName;
    [ObservableProperty] private string? _filterByCategoryName;
    [ObservableProperty] private bool? _filterByActiveStatus;
    [ObservableProperty] private decimal? _filterByMinPrice;
    [ObservableProperty] private decimal? _filterByMaxPrice;
    [ObservableProperty] private OrderByOptions _selectedOption;

    public bool CanGoToPreviousPage => CurrentPage > 1;
    public bool CanGoToNextPage => CurrentPage < _totalPages;

    public ProductsViewModel(
        IProductsApiService productsApiService,
        ICategoriesApiService categoriesApiService)
    {
        _productsApiService = productsApiService;
        _categoriesApiService = categoriesApiService;
        IsActive = true;
        LoadCategoriesCommand.ExecuteAsync(null);
        LoadProductsCommand.ExecuteAsync(null);
    }

    [RelayCommand]
    private async Task LoadCategoriesAsync()
    {
        var pagedResult = await _categoriesApiService.GetCategoriesAsync(new CategoryQueryParams());

        if (pagedResult is not null)
        {
            Categories.Clear();
            foreach (var category in pagedResult.Items)
            {
                Categories.Add(category);
            }
        }
    }
    
    [RelayCommand]
    private async Task LoadProductsAsync()
    {
        IsLoading = true;

        var queryParams = new ProductQueryParams
        {
            PageNumber = CurrentPage,
            PageSize = PageSize,
            OrderBy = SelectedOption,
            Name = FilterByName,
            CategoryName = SelectedCategory is null 
                ? string.Empty
                : SelectedCategory.Name,
            IsActive = FilterByActiveStatus,
            MinPrice = FilterByMinPrice,
            MaxPrice = FilterByMaxPrice
        };

        var pagedResult = await _productsApiService.GetProductsAsync(queryParams);

        if (pagedResult is not null)
        {
            Products.Clear();
            foreach (var product in pagedResult.Items)
            {
                Products.Add(product);
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
            await LoadProductsAsync();
        }
    }

    [RelayCommand]
    private async Task GoToPreviousPage()
    {
        if (CanGoToPreviousPage)
        {
            CurrentPage--;
            await LoadProductsAsync();
        }
    }

    partial void OnSelectedProductChanged(ProductResponse? value)
    {
        if (value is not null)
        {
            ProductSelected?.Invoke(value.Id);
        }
    }

    public void Receive(ProductRefreshMessage message)
    {
        LoadProductsCommand.Execute(null);
    }

}