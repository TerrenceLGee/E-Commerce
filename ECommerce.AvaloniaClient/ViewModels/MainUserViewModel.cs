using System;
using CommunityToolkit.Mvvm.ComponentModel;
using ECommerce.Shared.Enums;
using ECommerce.Shared.Enums.Extensions;
using System.Collections.ObjectModel;
using System.Linq;
using ECommerce.AvaloniaClient.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.AvaloniaClient.ViewModels;

public partial class MainUserViewModel : ObservableObject
{
    private readonly IServiceProvider _serviceProvider;
    private ProductsViewModel? _productsViewModelCache;
    private CategoriesViewModel? _categoriesViewModelCache;
    private AddressesViewModel? _addressesViewModelCache;
    private UsersViewModel? _usersViewModelCache;
    private SalesViewModel? _salesViewModelCache;
    private UserSalesViewModel? _userSalesViewModelCache;

    public ObservableCollection<MenuItemViewModel> MenuItems { get; } = [];
    [ObservableProperty] private ObservableObject? _currentSubView;
    [ObservableProperty] private MenuItemViewModel? _selectedMenuItem;
    public event Action? LogoutRequested;

    public MainUserViewModel(bool isAdmin, IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

        if (isAdmin)
        {
            var adminMenuItems = Enum.GetValues<AdminMenu>()
                .Select(e => new MenuItemViewModel(e.GetDisplayName(), e));

            foreach (var item in adminMenuItems)
            {
                MenuItems.Add(item);
            }
        }
        else
        {
            var customerMenuItems = Enum.GetValues<CustomerMenu>()
                .Select(e => new MenuItemViewModel(e.GetDisplayName(), e));

            foreach (var item in customerMenuItems)
            {
                MenuItems.Add(item);
            }
        }
    }

    partial void OnSelectedMenuItemChanged(MenuItemViewModel? value)
    {
        if (value is null)
        {
            return;
        }

        if (CurrentSubView is ObservableRecipient oldViewModel)
        {
            oldViewModel.IsActive = false;
        }

        switch (value.Value)
        {
            case AdminMenu.ViewAllCategories:
            case CustomerMenu.ViewAllCategories:
                if (_categoriesViewModelCache is null)
                {
                    _categoriesViewModelCache = _serviceProvider.GetRequiredService<CategoriesViewModel>();
                    _categoriesViewModelCache.CategorySelected += OnCategorySelected;
                }

                CurrentSubView = _categoriesViewModelCache;
                break;

            case AdminMenu.AddCategory:
                CurrentSubView = _serviceProvider
                    .GetRequiredService<AddCategoryViewModel>();
                break;

            case AdminMenu.UpdateCategory:
                CurrentSubView = _serviceProvider
                    .GetRequiredService<UpdateCategoryViewModel>();
                break;

            case AdminMenu.DeleteCategory:
                CurrentSubView = _serviceProvider
                    .GetRequiredService<DeleteCategoryViewModel>();
                break;

            case AdminMenu.ViewAllProducts:
            case CustomerMenu.ViewAllProducts:
                if (_productsViewModelCache is null)
                {
                    _productsViewModelCache = _serviceProvider.GetRequiredService<ProductsViewModel>();
                    _productsViewModelCache.ProductSelected += OnProductSelected;
                }

                CurrentSubView = _productsViewModelCache;
                break;

            case AdminMenu.AddProduct:
                CurrentSubView = _serviceProvider
                    .GetRequiredService<AddProductViewModel>();
                break;

            case AdminMenu.UpdateProduct:
                CurrentSubView = _serviceProvider
                    .GetRequiredService<UpdateProductViewModel>();
                break;

            case AdminMenu.DeleteProduct:
                CurrentSubView = _serviceProvider
                    .GetRequiredService<DeleteProductViewModel>();
                break;

            case AdminMenu.ViewSales:
                if (_salesViewModelCache is null)
                {
                    _salesViewModelCache = _serviceProvider.GetRequiredService<SalesViewModel>();
                    _salesViewModelCache.SaleSelected += OnSaleSelected;
                }

                CurrentSubView = _salesViewModelCache;
                break;

            case CustomerMenu.ViewMySales:
                if (_userSalesViewModelCache is null)
                {
                    _userSalesViewModelCache = _serviceProvider.GetRequiredService<UserSalesViewModel>();
                    _userSalesViewModelCache.SaleSelected += OnUserSaleSelected;
                }

                CurrentSubView = _userSalesViewModelCache;
                break;

            case AdminMenu.CreateSale:
            case CustomerMenu.CreateSale:
                CurrentSubView = _serviceProvider
                    .GetRequiredService<CreateSaleViewModel>();
                break;

            case AdminMenu.UpdateSale:
                CurrentSubView = _serviceProvider
                    .GetRequiredService<UpdateSaleViewModel>();
                break;

            case AdminMenu.RefundSale:
                CurrentSubView = _serviceProvider
                    .GetRequiredService<RefundSaleViewModel>();
                break;

            case AdminMenu.CancelSale:
                CurrentSubView = _serviceProvider
                    .GetRequiredService<CancelSaleViewModel>();
                break;

            case CustomerMenu.CancelSale:
                CurrentSubView = _serviceProvider
                    .GetRequiredService<UserCancelSaleViewModel>();
                break;

            case AdminMenu.ViewAllUsers:
                if (_usersViewModelCache is null)
                {
                    _usersViewModelCache = _serviceProvider.GetRequiredService<UsersViewModel>();
                    _usersViewModelCache.UserSelected += OnUserSelected;
                }

                CurrentSubView = _usersViewModelCache;
                break;

            case CustomerMenu.ViewAddresses:
                if (_addressesViewModelCache is null)
                {
                    _addressesViewModelCache = _serviceProvider.GetRequiredService<AddressesViewModel>();
                    _addressesViewModelCache.AddressSelected += OnAddressSelected;
                }

                CurrentSubView = _addressesViewModelCache;
                break;

            case CustomerMenu.AddAddress:
                CurrentSubView = _serviceProvider
                    .GetRequiredService<AddAddressViewModel>();
                break;

            case CustomerMenu.UpdateAddress:
                CurrentSubView = _serviceProvider
                    .GetRequiredService<UpdateAddressViewModel>();
                break;

            case CustomerMenu.DeleteAddress:
                CurrentSubView = _serviceProvider
                    .GetRequiredService<DeleteAddressViewModel>();
                break;

            case AdminMenu.Logout:
            case CustomerMenu.Logout:
                LogoutRequested?.Invoke();
                break;
        }
    }

    private async void OnSaleSelected(int saleId)
    {
        var saleDetailVM = _serviceProvider.GetRequiredService<SaleDetailViewModel>();

        saleDetailVM.BackRequested += OnBackRequestedFromSaleDetails;

        await saleDetailVM.InitializeAsync(saleId);

        CurrentSubView = saleDetailVM;
    }

    private async void OnUserSaleSelected(int saleId)
    {
        var userSaleDetailVM = _serviceProvider.GetRequiredService<UserSaleDetailViewModel>();

        userSaleDetailVM.BackRequested += OnBackRequestedFromUserSaleDetails;

        await userSaleDetailVM.InitializeAsync(saleId);

        CurrentSubView = userSaleDetailVM;
    }

    private async void OnUserSelected(string userId)
    {
        var userDetailVM = _serviceProvider.GetRequiredService<UserDetailViewModel>();

        userDetailVM.BackRequested += OnBackRequestedFromUserDetails;

        await userDetailVM.InitializeAsync(userId);

        CurrentSubView = userDetailVM;
    }

    private async void OnAddressSelected(int addressId)
    {
        var addressDetailVM = _serviceProvider.GetRequiredService<AddressDetailViewModel>();

        addressDetailVM.BackRequested += OnBackRequestedFromAddressDetails;

        await addressDetailVM.InitializeAsync(addressId);

        CurrentSubView = addressDetailVM;
    }

    private async void OnCategorySelected(int categoryId)
    {
        var categoryDetailVM = _serviceProvider
            .GetRequiredService<CategoryDetailViewModel>();

        categoryDetailVM.BackRequested += OnBackRequestedFromCategoryDetails;

        await categoryDetailVM.InitializeAsync(categoryId);

        CurrentSubView = categoryDetailVM;
    }

    private async void OnProductSelected(int productId)
    {
        var productDetailVM = _serviceProvider.GetRequiredService<ProductDetailViewModel>();

        productDetailVM.BackRequested += OnBackRequestedFromProductDetails;

        await productDetailVM.InitializeAsync(productId);

        CurrentSubView = productDetailVM;
    }

    private void OnBackRequestedFromUserSaleDetails()
    {
        CurrentSubView = _userSalesViewModelCache;
    }

    private void OnBackRequestedFromSaleDetails()
    {
        CurrentSubView = _salesViewModelCache;
    }

    private void OnBackRequestedFromAddressDetails()
    {
        CurrentSubView = _addressesViewModelCache;
    }

    private void OnBackRequestedFromCategoryDetails()
    {
        CurrentSubView = _categoriesViewModelCache;
    }

    private void OnBackRequestedFromProductDetails()
    {
        CurrentSubView = _productsViewModelCache;
    }

    private void OnBackRequestedFromUserDetails()
    {
        CurrentSubView = _usersViewModelCache;
    }
}