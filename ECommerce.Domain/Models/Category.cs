namespace ECommerce.Domain.Models;

public class Category : BaseEntity
{
    public ICollection<Product> Products { get; set; } = [];
}