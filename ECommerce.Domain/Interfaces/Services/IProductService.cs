using ECommerce.Shared.Dtos.Products.Request;
using ECommerce.Shared.Dtos.Products.Response;
using ECommerce.Shared.Dtos.Shared.Pagination;
using FluentResults;

namespace ECommerce.Domain.Interfaces.Services;

public interface IProductService
{
    Task<Result<ProductResponse>> CreateProductAsync(CreateProductRequest request);
    Task<Result<ProductResponse>> UpdateProductAsync(int id, UpdateProductRequest request);
    Task<Result<ProductResponse>> DeleteProductAsync(int id);
    Task<Result<ProductResponse>> GetProductByIdAsync(int id);
    Task<Result<PagedList<ProductResponse>>> GetAllProductsAsync(PaginationParams paginationParams);
}