using ECommerce.Domain.Interfaces.Services;
using ECommerce.Shared.Dtos.Categories.Request;
using ECommerce.Shared.Dtos.Categories.Response;
using ECommerce.Shared.Dtos.Shared.Pagination;
using ECommerce.Domain.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using ECommerce.Domain.Interfaces.Repositories;
using ECommerce.Infrastructure.Extensions;
using ECommerce.Shared.Enums;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<CategoryService> _logger;
    private readonly IMapper _mapper;

    public CategoryService(
        ICategoryRepository categoryRepository,
        ILogger<CategoryService> logger,
        IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _logger = logger;
        _mapper = mapper;
    }
    public async Task<Result<CategoryResponse>> CreateCategoryAsync(CreateCategoryRequest request)
    {
        try
        {
            var category = _mapper.Map<Category>(request);
            await _categoryRepository.AddAsync(category);
            return Result.Ok(_mapper.Map<CategoryResponse>(category));
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError("There was an error creating the category: {errorMessage}", ex.Message);
            return Result.Fail($"There was an error creating the category: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError("There was an unexpected error: {errorMessage}", ex.Message);
            return Result.Fail($"There was an unexpected error: {ex.Message}");
        }
    }

    public async Task<Result<CategoryResponse>> UpdateCategoryAsync(int id, UpdateCategoryRequest request)
    {
        try
        {
            var categoryToUpdate = await _categoryRepository.GetByIdAsync(id);

            if (categoryToUpdate is null)
            {
                _logger.LogError("Category with Id {id} not found", id);
                return Result.Fail($"Category with Id {id} not found");
            }

            _mapper.Map(request, categoryToUpdate);
            await _categoryRepository.UpdateAsync(categoryToUpdate);
            var updatedCategory = await _categoryRepository.GetByIdAsync(categoryToUpdate.Id);

            return Result.Ok(_mapper.Map<CategoryResponse>(updatedCategory));
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError("There was an error updating the category: {errorMessage}", ex.Message);
            return Result.Fail($"There was an unexpected error updating the category: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError("There was an unexpected error: {errorMessage}", ex.Message);
            return Result.Fail($"There was an unexpected error: {ex.Message}");
        }
    }

    public async Task<Result<CategoryResponse>> DeleteCategoryAsync(int id)
    {
        try
        {
            var categoryToDelete = await _categoryRepository.GetByIdAsync(id);

            if (categoryToDelete is null)
            {
                _logger.LogError("Category with Id {id} not found", id);
                return Result.Fail($"Category with Id {id} not found");
            }

            categoryToDelete.IsDeleted = true;
            await _categoryRepository.UpdateAsync(categoryToDelete);

            return Result.Ok(_mapper.Map<CategoryResponse>(categoryToDelete));
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError("There was an error deleting the category: {errorMessage}", ex.Message);
            return Result.Fail($"There was an unexpected error deleting the category: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError("There was an unexpected error: {errorMessage}", ex.Message);
            return Result.Fail($"There was an unexpected error: {ex.Message}");
        }
    }

    public async Task<Result<CategoryResponse>> GetCategoryByIdAsync(int id)
    {
        try
        {
            var category = await _categoryRepository.GetByIdAsync(id);

            if (category is null)
            {
                _logger.LogError("Category with Id {id} not found", id);
                return Result.Fail($"Category with Id {id} not found");
            }

            return Result.Ok(_mapper.Map<CategoryResponse>(category));
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError("There was an error retrieving the category with Id {id} : {errorMessage}", id, ex.Message);
            return Result.Fail($"There was an error retrieving the category with Id {id}: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError("There was an unexpected error: {errorMessage}", ex.Message);
            return Result.Fail($"There was an unexpected error: {ex.Message}");
        }
    }

    public async Task<Result<PagedList<CategoryResponse>>> GetAllCategoriesAsync(PaginationParams paginationParams)
    {
        try
        {
            var query = _categoryRepository.GetAllQueryable();
                

            if (!string.IsNullOrEmpty(paginationParams.Filter))
            {
                query = query.Where(c => c.Name.ToLower() == paginationParams.Filter.ToLower());
            }

            query = paginationParams.OrderBy switch
            {
                OrderByOptions.NameAsc => query.OrderBy(c => c.Name),
                OrderByOptions.NameDesc => query.OrderByDescending(c => c.Name),
                OrderByOptions.IdDesc => query.OrderByDescending(c => c.Id),
                _ => query.OrderBy(c => c.Id)
            };

            var projectedQuery = query.ProjectTo<CategoryResponse>(_mapper.ConfigurationProvider);

            var pagedResponse = await projectedQuery.ToPagedListAsync(
                paginationParams.PageNumber,
                paginationParams.PageSize);

            return Result.Ok(pagedResponse);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError("There was an error retrieving all categories: {errorMessage}", ex.Message);
            return Result.Fail($"There was an error all categories: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError("There was an unexpected error: {errorMessage}", ex.Message);
            return Result.Fail($"There was an unexpected error: {ex.Message}");
        }
    }
}