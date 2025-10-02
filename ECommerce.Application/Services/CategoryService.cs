using ECommerce.Domain.Interfaces.Services;
using ECommerce.Shared.Dtos.Categories.Request;
using ECommerce.Shared.Dtos.Categories.Response;
using ECommerce.Shared.Dtos.Shared.Pagination;
using FluentResults;

namespace ECommerce.Application.Services;

public class CategoryService : ICategoryService
{
    public async Task<Result<CategoryResponse>> CreateCategoryAsync(CreateCategoryRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<CategoryResponse>> UpdateCategoryAsync(int id, UpdateCategoryRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<CategoryResponse>> DeleteCategoryAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<CategoryResponse>> GetCategoryByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<PagedList<CategoryResponse>>> GetAllCategoriesAsync(PaginationParams paginationParams)
    {
        throw new NotImplementedException();
    }
}