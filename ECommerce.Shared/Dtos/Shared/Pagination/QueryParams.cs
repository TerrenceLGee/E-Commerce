using ECommerce.Shared.Enums;

namespace ECommerce.Shared.Dtos.Shared.Pagination;

public class QueryParams
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public OrderByOptions OrderBy { get; set; }
}