using System;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ECommerce.Shared.Dtos.Sales.Response;
using ECommerce.AvaloniaClient.Interfaces.Api;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ECommerce.Shared.Dtos.Shared.Pagination;
using ECommerce.AvaloniaClient.Messages;
using ECommerce.Shared.Enums;

namespace ECommerce.AvaloniaClient.ViewModels;

public partial class SalesViewModel : ObservableRecipient, IRecipient<SaleRefreshMessage>
{
    private readonly ISalesApiService _salesApiService;
    public ObservableCollection<SaleResponse> Sales { get; } = [];
    public event Action<int>? SaleSelected;

    public IEnumerable<OrderByOptions> SortOptions => new List<OrderByOptions>
    {
        OrderByOptions.DefaultOrder,
        OrderByOptions.DateAsc,
        OrderByOptions.DateDesc,
        OrderByOptions.CustomerIdAsc,
        OrderByOptions.CustomerIdDesc,
        OrderByOptions.IdDesc,
    };

    public IEnumerable<SaleStatus> SaleStatuses => Enum.GetValues<SaleStatus>();

    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private SaleResponse? _selectedSale;
    [ObservableProperty] private int _currentPage = 1;
    [ObservableProperty] private int _totalPages;
    [ObservableProperty] private int _pageSize = 10;
    [ObservableProperty] private int _totalCount;
    [ObservableProperty] private DateTimeOffset? _filterByCreatedAtOffset;
    [ObservableProperty] private DateTimeOffset? _filterByUpdatedAtOffset;
    [ObservableProperty] private string? _filterByCustomerId;
    [ObservableProperty] private decimal? _filterByMinPrice;
    [ObservableProperty] private decimal? _filterByMaxPrice;
    [ObservableProperty] private SaleStatus? _filterBySaleStatus;
    [ObservableProperty] private OrderByOptions _selectedOption;

    public DateTime? FilterByCreatedAt => FilterByCreatedAtOffset.HasValue
        ? FilterByCreatedAtOffset.Value.UtcDateTime
        : null;

    public DateTime? FilterByUpdatedAt => FilterByUpdatedAtOffset.HasValue
        ? FilterByUpdatedAtOffset.Value.UtcDateTime
        : null;

    public bool CanGoToPreviousPage => CurrentPage > 1;
    public bool CanGoToNextPage => CurrentPage < TotalPages;

    public SalesViewModel(ISalesApiService salesApiService)
    {
        _salesApiService = salesApiService;
        IsActive = true;
        LoadSalesCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadSalesAsync()
    {
        IsLoading = true;

        var queryParams = new SaleQueryParams
        {
            PageNumber = CurrentPage,
            PageSize = PageSize,
            OrderBy = SelectedOption,
            CreatedAt = FilterByCreatedAt,
            UpdatedAt = FilterByUpdatedAt,
            CustomerId = FilterByCustomerId,
            MinPrice = FilterByMinPrice,
            MaxPrice = FilterByMaxPrice,
            Status = FilterBySaleStatus
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
            await LoadSalesAsync();
        }
    }

    [RelayCommand]
    private async Task GoToPreviousPage()
    {
        if (CanGoToPreviousPage)
        {
            CurrentPage--;
            await LoadSalesAsync();
        }
    }

    partial void OnSelectedSaleChanged(SaleResponse? value)
    {
        if (value is not null)
        {
            SaleSelected?.Invoke(value.Id);
        }
    }

    public void Receive(SaleRefreshMessage message)
    {
        LoadSalesCommand.Execute(null);
    }
}