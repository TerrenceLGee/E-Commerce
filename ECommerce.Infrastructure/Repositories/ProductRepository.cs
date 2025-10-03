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

    public async Task<PagedList<Product>> GetAllAsync(PaginationParams paginationParams)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .AsQueryable();

        if (!string.IsNullOrEmpty(paginationParams.Filter))
        {
            query = query.Where(p => p.Category.Name.ToLower() == paginationParams.Filter.ToLower());
        }

        query = paginationParams.OrderBy switch
        {
            OrderByOptions.NameAsc => query.OrderBy(p => p.Name),
            OrderByOptions.NameDesc => query.OrderByDescending(p => p.Name),
            OrderByOptions.PriceAsc => query.OrderBy(p => p.Price),
            OrderByOptions.PriceDesc => query.OrderByDescending(p => p.Price),
            OrderByOptions.IdDesc => query.OrderByDescending(p => p.Id),
            _ => query.OrderBy(p => p.Id)
        };

        var products = await query.ToListAsync();
        var pagedProducts = new PagedList<Product>(
            products,
            products.Count,
            paginationParams.PageNumber,
            paginationParams.PageSize);

        return pagedProducts;
    }

    public async Task<List<Product>> GetByIdsAsync(IEnumerable<int> ids)
    {
        return await _context.Products
            .Where(p => ids.Contains(p.Id))
            .ToListAsync();
    }
}