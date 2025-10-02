using ECommerce.Shared.Enums;

namespace ECommerce.Shared.Dtos.Addresses.Request;

public class CreateAddressRequest
{
    public string StreetNumber { get; set; } = string.Empty;
    public string StreetName { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public AddressType AddressType { get; set; }
}