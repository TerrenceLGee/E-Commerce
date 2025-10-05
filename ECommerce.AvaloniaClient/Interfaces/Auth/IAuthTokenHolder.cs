namespace ECommerce.AvaloniaClient.Interfaces.Auth;

public interface IAuthTokenHolder
{
    string? Token { get; }
    void SetToken(string? token);
}