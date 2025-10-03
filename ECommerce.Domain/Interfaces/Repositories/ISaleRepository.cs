using ECommerce.Domain.Models;
using ECommerce.Shared.Dtos.Shared.Pagination;

namespace ECommerce.Domain.Interfaces.Repositories;

public interface ISaleRepository
{
    Task AddAsync(Sale sale);
    Task UpdateAsync(Sale sale);
    Task<Sale?> GetByIdAsync(int id);
    Task<Sale?> GetUserSaleByIdAsync(string userId, int saleId);
    Task<PagedList<Sale>> GetAllAsync(PaginationParams paginationParams);
    Task<PagedList<Sale>> GetAllByUserIdAsync(string userId, PaginationParams paginationParams);
}