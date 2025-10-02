using ECommerce.Domain.Models;
using ECommerce.Shared.Dtos.Shared.Pagination;

namespace ECommerce.Domain.Interfaces.Repositories;

public interface ICategoryRepository
{
    Task AddAsync(Category category);
    Task UpdateAsync(Category category);
    Task<bool> ExistsAsync(int id);
    Task<Category?> GetByIdAsync(int id);
    Task<PagedList<Category>> GetAllAsync(PaginationParams paginationParams);
}