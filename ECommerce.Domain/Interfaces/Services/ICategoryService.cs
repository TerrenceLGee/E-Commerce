using ECommerce.Shared.Dtos.Categories.Request;
using ECommerce.Shared.Dtos.Categories.Response;
using ECommerce.Shared.Dtos.Shared.Pagination;
using FluentResults;

namespace ECommerce.Domain.Interfaces.Services;

public interface ICategoryService
{
    Task<Result<CategoryResponse>> CreateCategoryAsync(CreateCategoryRequest request);
    Task<Result<CategoryResponse>> UpdateCategoryAsync(int id, UpdateCategoryRequest request);
    Task<Result<CategoryResponse>> DeleteCategoryAsync(int id);
    Task<Result<CategoryResponse>> GetCategoryByIdAsync(int id);
    Task<Result<PagedList<CategoryResponse>>> GetAllCategoriesAsync(PaginationParams paginationParams);
}