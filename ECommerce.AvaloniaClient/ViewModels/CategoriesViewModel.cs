using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ECommerce.AvaloniaClient.Interfaces.Api;
using ECommerce.Shared.Dtos.Categories.Response;
using ECommerce.Shared.Dtos.Shared.Pagination;
using ECommerce.AvaloniaClient.Messages;
using ECommerce.Shared.Enums;

namespace ECommerce.AvaloniaClient.ViewModels;

public partial class CategoriesViewModel : ObservableRecipient, IRecipient<CategoryRefreshMessage>
{
    private readonly ICategoriesApiService _categoriesApiService;
    public ObservableCollection<CategoryResponse> Categories { get; } = [];
    public event Action<int>? CategorySelected;

    public IEnumerable<OrderByOptions> SortOptions => new List<OrderByOptions>
    {
        OrderByOptions.DefaultOrder,
        OrderByOptions.NameAsc,
        OrderByOptions.NameDesc,
        OrderByOptions.IdDesc
    };

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private CategoryResponse? _selectedCategory;
    [ObservableProperty] private int _currentPage = 1;
    [ObservableProperty] private int _totalPages;
    [ObservableProperty] private int _pageSize = 10;
    [ObservableProperty] private int _totalCount;
    [ObservableProperty] private string? _filterByName;
    [ObservableProperty] private bool? _filterByActiveStatus;
    [ObservableProperty] private OrderByOptions _selectedOption;

    public bool CanGoToPreviousPage => CurrentPage > 1;
    public bool CanGoToNextPage => CurrentPage < _totalPages;

    public CategoriesViewModel(ICategoriesApiService categoriesApiService)
    {
        _categoriesApiService = categoriesApiService;
        IsActive = true;
        LoadCategoriesCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadCategoriesAsync()
    {
        IsLoading = true;

        var queryParams = new CategoryQueryParams
        {
            PageNumber = CurrentPage,
            PageSize = PageSize,
            OrderBy = SelectedOption,
            Name = FilterByName,
            IsActive = FilterByActiveStatus,
        };
        var pagedResult = await _categoriesApiService.GetCategoriesAsync(queryParams);

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

    public void Receive(CategoryRefreshMessage message)
    {
        LoadCategoriesCommand.Execute(null);
    }
}