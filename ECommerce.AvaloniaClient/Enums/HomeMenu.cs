using System.ComponentModel.DataAnnotations;

namespace ECommerce.AvaloniaClient.Enums;

public enum HomeMenu
{
    [Display(Name = "Login into your account")]
    Login,
    [Display(Name = "Register as a new customer")]
    Register,
    [Display(Name = "Log out/return to main menu")]
    Logout,
}