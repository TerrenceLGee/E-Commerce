using ECommerce.Shared.Enums;

namespace ECommerce.Shared.Dtos.Shared.Pagination;

public class SaleQueryParams : QueryParams
{
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; } 
    public string? CustomerId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public SaleStatus? Status { get; set; }
}