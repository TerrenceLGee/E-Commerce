using ECommerce.Domain.Interfaces.Services;
using ECommerce.Shared.Dtos.Products.Request;
using ECommerce.Shared.Dtos.Products.Response;
using ECommerce.Shared.Dtos.Shared.Pagination;
using FluentResults;

namespace ECommerce.Application.Services;

public class ProductService : IProductService
{
    public async Task<Result<ProductResponse>> CreateProductAsync(CreateProductRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<ProductResponse>> UpdateProductAsync(int id, UpdateProductRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<ProductResponse>> DeleteProductAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<ProductResponse>> GetProductByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<PagedList<ProductResponse>>> GetAllProductsAsync(PaginationParams paginationParams)
    {
        throw new NotImplementedException();
    }
}