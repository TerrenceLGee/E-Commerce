using System.ComponentModel.DataAnnotations;

namespace ECommerce.AvaloniaClient.Enums;

public enum CustomerMenu
{
    [Display(Name = "View all available categories")]
    ViewAllCategories,
    [Display(Name = "View all available products")]
    ViewAllProducts,
    [Display(Name = "View all of my sales")]
    ViewMySales,
    [Display(Name = "Purchase products")]
    CreateSale,
    [Display(Name = "Cancel an existing sale")]
    CancelSale,
    [Display(Name = "View all of my addresses")]
    ViewAddresses,
    [Display(Name = "Add a new address to the system")]
    AddAddress,
    [Display(Name = "Update an existing address in the system")]
    UpdateAddress,
    [Display(Name = "Delete an existing address in the system")]
    DeleteAddress,
    [Display(Name = "Logout of your account")]
    Logout,
}