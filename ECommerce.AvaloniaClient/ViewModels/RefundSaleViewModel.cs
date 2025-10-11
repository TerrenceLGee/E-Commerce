using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ECommerce.Shared.Dtos.Sales.Response;
using ECommerce.Shared.Dtos.Shared.Pagination;
using ECommerce.AvaloniaClient.Interfaces.Api;
using ECommerce.AvaloniaClient.Messages;

namespace ECommerce.AvaloniaClient.ViewModels;

public partial class RefundSaleViewModel : ObservableObject
{
    private readonly ISalesApiService _salesApiService;
    public ObservableCollection<SaleResponse> Sales { get; } = [];

    [ObservableProperty] private SaleResponse? _selectedSale;
    [ObservableProperty] private int _currentPage = 1;
    [ObservableProperty] private int _totalPages;
    [ObservableProperty] private int _pageSize = 10;
    [ObservableProperty] private int _totalCount;
    [ObservableProperty] private string? _successMessage;
    [ObservableProperty] private string? _errorMessage;

    public bool CanGoToPreviousPage => CurrentPage > 1;
    public bool CanGoToNextPage => CurrentPage < TotalPages;

    public RefundSaleViewModel(ISalesApiService salesApiService)
    {
        _salesApiService = salesApiService;
        InitializeCommand.ExecuteAsync(null);
    }

    [RelayCommand]
    private async Task InitializeAsync()
    {
        var queryParams = new SaleQueryParams { PageNumber = CurrentPage, PageSize = PageSize };

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
        
        OnPropertyChanged(nameof(CanGoToPreviousPage));
        OnPropertyChanged(nameof(CanGoToNextPage));
    }

    [RelayCommand]
    private async Task RefundSale()
    {
        var id = SelectedSale!.Id;

        var success = await _salesApiService.RefundSaleAsync(id);

        if (success)
        {
            ErrorMessage = null;
            SuccessMessage = $"Sale with Id {id} refunded successfully!";
            SelectedSale = null;
            WeakReferenceMessenger
                .Default
                .Send(new SaleRefreshMessage());
        }
        else
        {
            SuccessMessage = null;
            ErrorMessage = "Unable to refund sale";
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
}