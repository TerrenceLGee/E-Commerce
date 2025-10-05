using ECommerce.Domain.Models;
using ECommerce.Shared.Dtos.Shared.Pagination;

namespace ECommerce.Domain.Interfaces.Repositories;

public interface IAddressRepository
{
    Task AddAsync(Address address);
    Task UpdateAsync(Address address);
    Task DeleteAsync(Address address);
    IQueryable<Address> GetAllQueryable(string customerId);
    Task<Address?> GetByIdAsync(string customerId, int addressID);
}