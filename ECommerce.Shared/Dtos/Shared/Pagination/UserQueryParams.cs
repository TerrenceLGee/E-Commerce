namespace ECommerce.Shared.Dtos.Shared.Pagination;

public class UserQueryParams : QueryParams
{
    public string? UserId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? EmailAddress { get; set; }
    public DateOnly? BirthDate { get; set; }
}