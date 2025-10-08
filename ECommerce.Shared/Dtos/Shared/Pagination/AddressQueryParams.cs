using ECommerce.Shared.Enums;

namespace ECommerce.Shared.Dtos.Shared.Pagination;

public class AddressQueryParams : QueryParams
{
    public string? StreetNumber { get; set; }
    public string? StreetName { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public string? Country { get; set; }
    public AddressType? AddressType { get; set; }
}