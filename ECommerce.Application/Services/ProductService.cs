using ECommerce.Domain.Interfaces.Services;
using ECommerce.Shared.Dtos.Shared.Pagination;
using ECommerce.Domain.Models;
using AutoMapper;
using ECommerce.Domain.Interfaces.Repositories;
using ECommerce.Shared.Dtos.Products.Request;
using ECommerce.Shared.Dtos.Products.Response;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ILogger<ProductService> _logger;
    private readonly IMapper _mapper;

    public ProductService(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        ILogger<ProductService> logger,
        IMapper mapper)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _logger = logger;
        _mapper = mapper;
    }
    public async Task<Result<ProductResponse>> CreateProductAsync(CreateProductRequest request)
    {
        try
        {
            var categoryId = request.CategoryId;
            
            if (!await _categoryRepository.ExistsAsync(categoryId))
            {
                _logger.LogError("Category with Id {id} not found, cannot add new product", categoryId);
                return Result.Fail($"Category with Id {categoryId} not found, cannot add new product");
            }
            var product = _mapper.Map<Product>(request);
            await _productRepository.AddAsync(product);
            return Result.Ok(_mapper.Map<ProductResponse>(product));
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError("There was an error creating the product: {errorMessage}", ex.Message);
            return Result.Fail($"There was an error creating the product: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError("There was an unexpected error: {errorMessage}", ex.Message);
            return Result.Fail($"There was an unexpected error: {ex.Message}");
        }
    }

    public async Task<Result<ProductResponse>> UpdateProductAsync(int id, UpdateProductRequest request)
    {
        try
        {
            var categoryId = request.CategoryId;

            if (!await _categoryRepository.ExistsAsync(categoryId))
            {
                _logger.LogError("Category with Id {id} not found, cannot update product", categoryId);
                return Result.Fail($"Category with Id {categoryId} not found, cannot update product");
            }
            
            var productToUpdate = await _productRepository.GetByIdAsync(id);

            if (productToUpdate is null)
            {
                _logger.LogError("Product with Id {id} not found", id);
                return Result.Fail($"Product with Id {id} not found");
            }

            _mapper.Map(request, productToUpdate);
            await _productRepository.UpdateAsync(productToUpdate);
            var updatedProduct = await _productRepository.GetByIdAsync(productToUpdate.Id);
            return Result.Ok(_mapper.Map<ProductResponse>(updatedProduct));
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError("There was an error updating the product: {errorMessage}", ex.Message);
            return Result.Fail($"There was an unexpected error updating the product: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError("There was an unexpected error: {errorMessage}", ex.Message);
            return Result.Fail($"There was an unexpected error: {ex.Message}");
        }
    }

    public async Task<Result<ProductResponse>> DeleteProductAsync(int id)
    {
        try
        {
            var productToDelete = await _productRepository.GetByIdAsync(id);

            if (productToDelete is null)
            {
                _logger.LogError("Product with Id {id} not found", id);
                return Result.Fail($"Product with Id {id} not found");
            }

            productToDelete.IsDeleted = true;
            await _productRepository.UpdateAsync(productToDelete);
            return Result.Ok(_mapper.Map<ProductResponse>(productToDelete));
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError("There was an error deleting the product: {errorMessage}", ex.Message);
            return Result.Fail($"There was an unexpected error deleting the product: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError("There was an unexpected error: {errorMessage}", ex.Message);
            return Result.Fail($"There was an unexpected error: {ex.Message}");
        }
    }

    public async Task<Result<ProductResponse>> GetProductByIdAsync(int id)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(id);

            if (product is null)
            {
                _logger.LogError("Product with Id {id} not found", id);
                return Result.Fail($"Product with Id {id} not found");
            }

            return Result.Ok(_mapper.Map<ProductResponse>(product));
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError("There was an error retrieving the product with Id {id} : {errorMessage}", id, ex.Message);
            return Result.Fail($"There was an error retrieving the product with Id {id}: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError("There was an unexpected error: {errorMessage}", ex.Message);
            return Result.Fail($"There was an unexpected error: {ex.Message}");
        }
    }

    public async Task<Result<PagedList<ProductResponse>>> GetAllProductsAsync(PaginationParams paginationParams)
    {
        try
        {
            var pagedProducts = await _productRepository.GetAllAsync(paginationParams);

            var pagedResponse = _mapper.Map<PagedList<ProductResponse>>(pagedProducts);
            
            return Result.Ok(pagedResponse);
        }
        catch (ArgumentNullException ex)
        {
            _logger.LogError("There was an error retrieving all products: {errorMessage}", ex.Message);
            return Result.Fail($"There was an error all products: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError("There was an unexpected error: {errorMessage}", ex.Message);
            return Result.Fail($"There was an unexpected error: {ex.Message}");
        }
    }
}