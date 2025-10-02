using ECommerce.Domain.Models;
using ECommerce.Shared.Dtos.Shared.Pagination;

namespace ECommerce.Domain.Interfaces.Repositories;

public interface IAddressRepository
{
    Task AddAsync(Address address);
    Task UpdateAsync(Address address);
    Task DeleteAsync(Address address);
    Task<PagedList<Address>> GetAllAddressesAsync(string customerId, PaginationParams paginationParams);
    Task<Address?> GetByIdAsync(string customerId, int addressID);
}