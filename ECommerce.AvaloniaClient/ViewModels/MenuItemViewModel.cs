using System;

namespace ECommerce.AvaloniaClient.ViewModels;

public class MenuItemViewModel
{
    public string DisplayName { get; set; }
    public Enum Value { get; set; }

    public MenuItemViewModel(string displayName, Enum value)
    {
        DisplayName = displayName;
        Value = value;
    }
}