using System.ComponentModel.DataAnnotations;

namespace ECommerce.AvaloniaClient.Enums;

public enum AdminMenu
{
    [Display(Name = "View all available categories")]
    ViewAllCategories,
    [Display(Name = "Add a category to the database")]
    AddCategory,
    [Display(Name = "Update an existing category in the database")]
    UpdateCategory,
    [Display(Name = "Delete an existing category in the database")]
    DeleteCategory,
    [Display(Name = "View all available products")]
    ViewAllProducts,
    [Display(Name = "Add a product to the database")]
    AddProduct,
    [Display(Name = "Update an existing product in the database")]
    UpdateProduct,
    [Display(Name = "Delete an existing product in the database")]
    DeleteProduct,
    [Display(Name = "View all sales")]
    ViewSales,
    [Display(Name = "Purchase products")]
    CreateSale,
    [Display(Name = "Update the status of an existing sale")]
    UpdateSale,
    [Display(Name = "Refund an existing sale")]
    RefundSale,
    [Display(Name = "Cancel an existing sale")]
    CancelSale,
    [Display(Name = "View all registered users in the system")]
    ViewAllUsers,
    [Display(Name = "Logout of your account")]
    Logout,
}