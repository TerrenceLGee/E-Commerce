using ECommerce.Domain.Interfaces.Repositories;
using ECommerce.Domain.Models;
using ECommerce.Infrastructure.Data;
using ECommerce.Shared.Dtos.Shared.Pagination;
using ECommerce.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public IQueryable<Product> GetAllQueryable()
    {
        return _context.Products
            .Include(p => p.Category)
            .AsQueryable();
    }

    public IQueryable<Product> GetAllByCategoryIdQueryable(int categoryId)
    {
        return _context.Products
            .Where(p => p.CategoryId == categoryId)
            .Include(p => p.Category)
            .AsQueryable();
    }


    public async Task<List<Product>> GetByIdsAsync(IEnumerable<int> ids)
    {
        return await _context.Products
            .Where(p => ids.Contains(p.Id))
            .ToListAsync();
    }
}