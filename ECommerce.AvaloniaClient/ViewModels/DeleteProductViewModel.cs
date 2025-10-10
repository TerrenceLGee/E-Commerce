using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ECommerce.AvaloniaClient.Interfaces.Api;
using ECommerce.AvaloniaClient.Messages;
using ECommerce.Shared.Dtos.Products.Response;
using ECommerce.Shared.Dtos.Shared.Pagination;

namespace ECommerce.AvaloniaClient.ViewModels;

public partial class DeleteProductViewModel : ObservableObject
{
    private readonly IProductsApiService _productsApiService;
    public ObservableCollection<ProductResponse> Products { get; } = [];
    [ObservableProperty] private ProductResponse? _selectedProduct;
    [ObservableProperty] private ProductResponse? _deletedProduct;
    [ObservableProperty] private string? _errorMessage;
    [ObservableProperty] private int _currentPage = 1;
    [ObservableProperty] private int _totalPages;
    [ObservableProperty] private int _pageSize = 10;
    [ObservableProperty] private int _totalCount;
    public bool IsInConfirmationMode => DeletedProduct is not null;

    public bool CanGoToPreviousPage => CurrentPage > 1;
    public bool CanGoToNextPage => CurrentPage < TotalPages;

    public DeleteProductViewModel(IProductsApiService productsApiService)
    {
        _productsApiService = productsApiService;
        InitializeCommand.ExecuteAsync(null);
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        var queryParams = new ProductQueryParams { PageNumber = CurrentPage, PageSize = PageSize };
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
        
        OnPropertyChanged(nameof(CanGoToPreviousPage));
        OnPropertyChanged(nameof(CanGoToNextPage));
    }

    [RelayCommand]
    private async Task DeleteProduct()
    {
        ErrorMessage = null;

        var id = SelectedProduct!.Id;
        DeletedProduct = await _productsApiService.DeleteProductAsync(id);

        if (DeletedProduct is not null)
        {
            ErrorMessage = null;
            SelectedProduct = null;
            WeakReferenceMessenger
                .Default
                .Send(new ProductRefreshMessage());
        }
        else
        {
            ErrorMessage = "Unable to delete product";
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
    private async Task DeleteAnotherProduct()
    {
        DeletedProduct = null;
        await InitializeAsync();
    }
}