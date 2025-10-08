using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ECommerce.AvaloniaClient.Interfaces.Api;
using ECommerce.Shared.Dtos.Categories.Response;
using ECommerce.Shared.Dtos.Shared.Pagination;
using ECommerce.AvaloniaClient.Messages;

namespace ECommerce.AvaloniaClient.ViewModels;

public partial class CategoriesViewModel : ObservableObject, IRecipient<CategoryAddedMessage>
{
    private readonly ICategoriesApiService _categoriesApiService;
    public ObservableCollection<CategoryResponse> Categories { get; } = [];
    public event Action<int>? CategorySelected;

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private CategoryResponse? _selectedCategory;
    [ObservableProperty] private int _currentPage = 1;
    [ObservableProperty] private int _totalPages;
    [ObservableProperty] private int _pageSize = 10;
    [ObservableProperty] private int _totalCount;

    public bool CanGoToPreviousPage => CurrentPage > 1;
    public bool CanGoToNextPage => CurrentPage < _totalPages;

    public CategoriesViewModel(ICategoriesApiService categoriesApiService)
    {
        _categoriesApiService = categoriesApiService;
        WeakReferenceMessenger.Default.Register(this);
        LoadCategoriesCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadCategoriesAsync()
    {
        IsLoading = true;

        var paginationParams = new PaginationParams { PageNumber = CurrentPage, PageSize = PageSize };
        var pagedResult = await _categoriesApiService.GetCategoriesAsync(paginationParams);

        if (pagedResult is not null)
        {
            Categories.Clear();
            foreach (var category in pagedResult.Items)
            {
                Categories.Add(category);
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
            await LoadCategoriesAsync();
        }
    }

    [RelayCommand]
    private async Task GoToPreviousPage()
    {
        if (CanGoToPreviousPage)
        {
            CurrentPage--;
            await LoadCategoriesAsync();
        }
    }

    partial void OnSelectedCategoryChanged(CategoryResponse? value)
    {
        if (value is not null)
        {
            CategorySelected?.Invoke(value.Id);
        }
    }

    public void Receive(CategoryAddedMessage message)
    {
        LoadCategoriesCommand.Execute(null);
    }
}