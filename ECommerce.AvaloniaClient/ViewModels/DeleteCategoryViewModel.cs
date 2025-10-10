using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ECommerce.AvaloniaClient.Interfaces.Api;
using ECommerce.AvaloniaClient.Messages;
using ECommerce.Shared.Dtos.Categories.Response;
using ECommerce.Shared.Dtos.Shared.Pagination;

namespace ECommerce.AvaloniaClient.ViewModels;

public partial class DeleteCategoryViewModel : ObservableObject
{
    private readonly ICategoriesApiService _categoriesApiService;
    public ObservableCollection<CategoryResponse> Categories { get; } = [];
    [ObservableProperty] private CategoryResponse? _selectedCategory;
    [ObservableProperty] private CategoryResponse? _deletedCategory;
    [ObservableProperty] private string? _errorMessage;
    [ObservableProperty] private int _currentPage = 1;
    [ObservableProperty] private int _totalPages;
    [ObservableProperty] private int _pageSize = 10;
    [ObservableProperty] private int _totalCount;
    public bool IsInConfirmationMode => DeletedCategory is not null;

    public bool CanGoToPreviousPage => CurrentPage > 1;
    public bool CanGoToNextPage => CurrentPage < TotalPages;

    public DeleteCategoryViewModel(ICategoriesApiService categoriesApiService)
    {
        _categoriesApiService = categoriesApiService;
        InitializeCommand.ExecuteAsync(null);
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        var queryParams = new CategoryQueryParams { PageNumber = CurrentPage, PageSize = PageSize };
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
        
        OnPropertyChanged(nameof(CanGoToPreviousPage));
        OnPropertyChanged(nameof(CanGoToNextPage));
    }

    [RelayCommand]
    private async Task DeleteCategory()
    {
        ErrorMessage = null;

        var id = SelectedCategory!.Id;
        DeletedCategory = await _categoriesApiService.DeleteCategoryAsync(id);

        if (DeletedCategory is not null)
        {
            ErrorMessage = null;
            SelectedCategory = null;
            WeakReferenceMessenger
                .Default
                .Send(new CategoryRefreshMessage());
        }
        else
        {
            ErrorMessage = "Unable to delete category";
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
    private async Task DeleteAnotherCategory()
    {
        DeletedCategory = null;
        await InitializeAsync();
    }
}