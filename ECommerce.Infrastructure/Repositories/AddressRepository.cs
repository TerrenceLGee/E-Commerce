using ECommerce.Domain.Interfaces.Repositories;
using ECommerce.Domain.Models;
using ECommerce.Infrastructure.Data;
using ECommerce.Shared.Dtos.Shared.Pagination;
using ECommerce.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories;

public class AddressRepository : IAddressRepository
{
    private readonly ApplicationDbContext _context;

    public AddressRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task AddAsync(Address address)
    {
        await _context.Addresses.AddAsync(address);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Address address)
    {
        _context.Addresses.Update(address);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Address address)
    {
        _context.Addresses.Remove(address);
        await _context.SaveChangesAsync();
    }

    public IQueryable<Address> GetAllQueryable(string customerId)
    {
        return _context.Addresses
            .Where(a => a.CustomerId == customerId)
            .AsQueryable();
    }

    public async Task<Address?> GetByIdAsync(string customerId, int addressId)
    {
        return await _context.Addresses
            .Where(a => a.CustomerId == customerId)
            .FirstOrDefaultAsync(a => a.Id == addressId);
    }
}