using ECommerce.Domain.Interfaces.Repositories;
using ECommerce.Domain.Models;
using ECommerce.Infrastructure.Data;
using ECommerce.Shared.Dtos.Shared.Pagination;
using ECommerce.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ApplicationDbContext _context;

    public CategoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task AddAsync(Category category)
    {
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Category category)
    {
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Categories
            .AnyAsync(c => c.Id == id);
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        return await _context.Categories
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<PagedList<Category>> GetAllAsync(PaginationParams paginationParams)
    {
        var query = _context.Categories
            .Include(c => c.Products)
            .AsQueryable();

        if (!string.IsNullOrEmpty(paginationParams.Filter))
        {
            query = query.Where(c => c.Name.ToLower() == paginationParams.Filter.ToLower());
        }

        query = paginationParams.OrderBy switch
        {
            OrderByOptions.NameAsc => query.OrderBy(c => c.Name),
            OrderByOptions.NameDesc => query.OrderByDescending(c => c.Name),
            _ => query.OrderBy(c => c.Id)
        };

        var categories = await query.ToListAsync();
        var pagedCategories = new PagedList<Category>(
            categories,
            categories.Count,
            paginationParams.PageNumber,
            paginationParams.PageSize);

        return pagedCategories;
    }
}