using ECommerce.AvaloniaClient.Interfaces.Auth;

namespace ECommerce.AvaloniaClient.Services;

public class AuthTokenHolder : IAuthTokenHolder
{
    public string? Token { get; private set; }
    public void SetToken(string? token) => Token = token;
}