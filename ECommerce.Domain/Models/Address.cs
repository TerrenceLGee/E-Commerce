using ECommerce.Shared.Enums;

namespace ECommerce.Domain.Models;

public class Address
{
    public int Id { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public string StreetNumber { get; set; } = string.Empty;
    public string StreetName { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public AddressType AddressType { get; set; }
}