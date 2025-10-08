namespace ECommerce.Shared.Dtos.Shared.Pagination;

public class ProductQueryParams : QueryParams
{
    public string? Name { get; set; }
    public string? CategoryName { get; set; }
    public bool? IsActive { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
}