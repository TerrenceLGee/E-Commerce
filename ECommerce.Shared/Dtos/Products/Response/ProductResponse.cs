using ECommerce.Shared.Dtos.Categories.Response;
using ECommerce.Shared.Enums;

namespace ECommerce.Shared.Dtos.Products.Response;

public class ProductResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? StockKeepingUnit { get; set; }
    public decimal Price { get; set; }
    public DiscountStatus Discount { get; set; }
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; }
    public ProductCategoryResponse Category { get; set; } = null!;
}