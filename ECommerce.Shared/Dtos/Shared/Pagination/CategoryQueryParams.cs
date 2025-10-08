namespace ECommerce.Shared.Dtos.Shared.Pagination;

public class CategoryQueryParams : QueryParams
{
    public string? Name { get; set; }
    public bool? IsActive { get; set; }
}