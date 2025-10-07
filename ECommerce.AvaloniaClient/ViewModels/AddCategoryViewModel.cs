using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ECommerce.Shared.Dtos.Categories.Request;
using ECommerce.AvaloniaClient.Interfaces.Api;
using ECommerce.AvaloniaClient.Messages;
using ECommerce.Shared.Dtos.Categories.Response;

namespace ECommerce.AvaloniaClient.ViewModels;

public partial class AddCategoryViewModel : ObservableValidator
{
    private readonly ICategoriesApiService _categoriesApiService;

    public AddCategoryViewModel(ICategoriesApiService categoriesApiService)
    {
        _categoriesApiService = categoriesApiService;
    }

    [ObservableProperty]
    [Required(ErrorMessage = "Category name is required")]
    [NotifyPropertyChangedFor(nameof(CategoryNameErrors))]
    private string _categoryName = String.Empty;

    public string? CategoryNameErrors => GetErrors(nameof(CategoryName))
        .FirstOrDefault()?
        .ErrorMessage;

    [ObservableProperty]
    [MaxLength(500, ErrorMessage = "Description can be no longer than 500 characters")]
    [NotifyPropertyChangedFor(nameof(DescriptionErrors))]
    private string? _description;

    public string? DescriptionErrors => GetErrors(nameof(Description))
        .FirstOrDefault()?
        .ErrorMessage;

    [ObservableProperty] private bool _isActive;
    [ObservableProperty] private string? _errorMessage;
    [ObservableProperty] private CategoryResponse? _addedCategory;
    public bool IsInConfirmationMode => AddedCategory is not null;

    [RelayCommand]
    private async Task AddCategory()
    {
        ErrorMessage = null;
        ClearErrors();
        ValidateAllProperties();

        if (HasErrors)
        {
            return;
        }

        var createCategoryRequest = new CreateCategoryRequest
        {
            Name = CategoryName,
            Description = Description,
            IsActive = IsActive,
        };

        AddedCategory = await _categoriesApiService.CreateCategoryAsync(createCategoryRequest);

        if (AddedCategory is not null)
        {
            ErrorMessage = null;
            WeakReferenceMessenger
                .Default
                .Send(new CategoryAddedMessage());
        }
        else
        {
            ErrorMessage = "Unable to add category";
        }
    }

    [RelayCommand]
    private void AddAnotherCategory()
    {
        AddedCategory = null;
        CategoryName = string.Empty;
        Description = string.Empty;
        IsActive = false;
    }
}