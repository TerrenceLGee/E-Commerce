using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ECommerce.AvaloniaClient.Interfaces.Api;
using ECommerce.Shared.Dtos.Categories.Response;

namespace ECommerce.AvaloniaClient.ViewModels;

public partial class CategoryDetailViewModel : ObservableObject
{
    private readonly ICategoriesApiService _categoriesApiService;
    public event Action? BackRequested;

    [ObservableProperty] private CategoryResponse? _selectedCategory;

    public CategoryDetailViewModel(ICategoriesApiService categoriesApiService)
    {
        _categoriesApiService = categoriesApiService;
    }

    [RelayCommand]
    private void GoBack()
    {
        BackRequested?.Invoke();
    }

    public async Task InitializeAsync(int categoryId)
    {
        SelectedCategory = await _categoriesApiService.GetCategoryByIdAsync(categoryId);
    }
}