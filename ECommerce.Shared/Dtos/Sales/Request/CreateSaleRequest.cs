namespace ECommerce.Shared.Dtos.Sales.Request;

public class CreateSaleRequest
{
    public string StreetNumber { get; set; } = string.Empty;
    public string StreetName { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public List<SaleItemRequest> Items { get; set; } = [];
}