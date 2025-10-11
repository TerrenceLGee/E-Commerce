using System;
using CommunityToolkit.Mvvm.ComponentModel;
using ECommerce.Shared.Dtos.Products.Response;
using ECommerce.AvaloniaClient.Interfaces.Api;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;

namespace ECommerce.AvaloniaClient.ViewModels;

public partial class ProductDetailViewModel : ObservableObject
{
    private readonly IProductsApiService _productsApiService;
    public event Action? BackRequested;

    [ObservableProperty] private ProductResponse? _selectedProduct;

    public ProductDetailViewModel(IProductsApiService productsApiService)
    {
        _productsApiService = productsApiService;
    }

    [RelayCommand]
    private void GoBack()
    {
        BackRequested?.Invoke();
    }

    public async Task InitializeAsync(int productId)
    {
        SelectedProduct = await _productsApiService.GetProductByIdAsync(productId);
    }
}