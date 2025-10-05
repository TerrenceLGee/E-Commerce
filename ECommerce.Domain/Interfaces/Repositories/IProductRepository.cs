using ECommerce.Domain.Models;
using ECommerce.Shared.Dtos.Shared.Pagination;

namespace ECommerce.Domain.Interfaces.Repositories;

public interface IProductRepository
{
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task<Product?> GetByIdAsync(int id);
    IQueryable<Product> GetAllQueryable();
    Task<List<Product>> GetByIdsAsync(IEnumerable<int> ids);
}