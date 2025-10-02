using ECommerce.Shared.Dtos.Addresses.Response;

namespace ECommerce.Shared.Dtos.Auth.Response;

public class UserResponse
{
    public string UserId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
    public DateOnly BirthDate { get; set; }
    public int Age { get; set; }
    public List<AddressResponse> Addresses { get; set; } = [];
}