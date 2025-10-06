using System.Security.Claims;
using System.Threading.Tasks;
using ECommerce.Shared.Dtos.Auth.Request;

namespace ECommerce.AvaloniaClient.Interfaces.Api;

public interface ILoginApiService
{
    string? JwtToken { get; }
    Task<bool> LoginAsync(LoginRequest request);
    Task<bool> RegisterAsync(RegisterRequest request);
    ClaimsPrincipal? GetPrincipalFromToken(string token);
}