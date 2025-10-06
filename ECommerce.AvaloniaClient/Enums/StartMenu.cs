using System.ComponentModel.DataAnnotations;

namespace ECommerce.AvaloniaClient.Enums;

public enum StartMenu
{
    [Display(Name = "Go to login/registration page")]
    GoToHomePage,
    [Display(Name = "Exit the program")]
    ExitProgram,
}