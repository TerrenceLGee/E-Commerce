using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ECommerce.AvaloniaClient.Interfaces.Api;
using ECommerce.AvaloniaClient.Messages;
using ECommerce.Shared.Dtos.Sales.Response;
using ECommerce.Shared.Dtos.Shared.Pagination;

namespace ECommerce.AvaloniaClient.ViewModels;

public partial class CancelSaleViewModel : ObservableObject
{
    private readonly ISalesApiService _salesApiService;
    public ObservableCollection<SaleResponse> Sales { get; } = [];

    public CancelSaleViewModel(ISalesApiService salesApiService)
    {
        _salesApiService = salesApiService;
        InitializeCommand.ExecuteAsync(null);
    }

    [ObservableProperty] private SaleResponse? _selectedSale;
    [ObservableProperty] private string? _successMessage;
    [ObservableProperty] private string? _errorMessage;
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private int _currentPage = 1;
    [ObservableProperty] private int _totalPages;
    [ObservableProperty] private int _pageSize = 10;
    [ObservableProperty] private int _totalCount;

    public bool CanGoToPreviousPage => CurrentPage > 1;
    public bool CanGoToNextPage => CurrentPage < _totalPages;

    [RelayCommand]
    private async Task InitializeAsync()
    {
        IsLoading = true;

        var queryParams = new SaleQueryParams
        {
            PageNumber = CurrentPage,
            PageSize = PageSize
        };
        
        var pagedResult = await _salesApiService.GetAllSalesAsync(queryParams);

        if (pagedResult is not null)
        {
            Sales.Clear();
            foreach (var sale in pagedResult.Items)
            {
                Sales.Add(sale);
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
    private async Task CancelSale()
    {
        var id = SelectedSale!.Id;

        var success = await _salesApiService.CancelSaleAsync(id);

        if (success)
        {
            ErrorMessage = null;
            SuccessMessage = "Sale canceled successfully!";
            SelectedSale = null;
            WeakReferenceMessenger
                .Default
                .Send(new SaleAddedMessage());
        }
        else
        {
            SuccessMessage = null;
            ErrorMessage = "Unable to cancel sale";
        }
    }
}