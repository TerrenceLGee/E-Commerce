namespace ECommerce.Domain.Interfaces.Services;

public interface ITokenService
{
    Task<string> CreateTokenAsync(string userId, string email, IEnumerable<string> roles);
}