using ECommerce.Shared.Enums;

namespace ECommerce.Shared.Dtos.Products.Request;

public class CreateProductRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? StockKeepingUnit { get; set; }
    public decimal Price { get; set; }
    public DiscountStatus Discount { get; set; }
    public int StockQuantity { get; set; }
    public int CategoryId { get; set; }
}