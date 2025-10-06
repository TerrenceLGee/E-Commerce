using System.Collections.Generic;
using System.Threading.Tasks;
using ECommerce.Shared.Dtos.Categories.Request;
using ECommerce.Shared.Dtos.Categories.Response;
using ECommerce.Shared.Dtos.Shared.Pagination;

namespace ECommerce.AvaloniaClient.Interfaces.Api;

public interface ICategoriesApiService
{
    Task<CategoryResponse?> CreateCategoryAsync(CreateCategoryRequest request);
    Task<CategoryResponse?> UpdateCategoryAsync(int id, UpdateCategoryRequest request);
    Task<CategoryResponse?> DeleteCategoryAsync(int id);
    Task<PagedList<CategoryResponse>?> GetCategoriesAsync(PaginationParams paginationParams);
    Task<CategoryResponse?> GetCategoryByIdAsync(int id);
}