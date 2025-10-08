using System.Threading.Tasks;
using ECommerce.Shared.Dtos.Products.Request;
using ECommerce.Shared.Dtos.Products.Response;
using ECommerce.Shared.Dtos.Shared.Pagination;

namespace ECommerce.AvaloniaClient.Interfaces.Api;

public interface IProductsApiService
{
    Task<ProductResponse?> CreateProductAsync(CreateProductRequest request);
    Task<ProductResponse?> UpdateProductAsync(int id, UpdateProductRequest request);
    Task<ProductResponse?> DeleteProductAsync(int id);
    Task<PagedList<ProductResponse>?> GetProductsAsync(ProductQueryParams queryParams);
    Task<PagedList<ProductResponse>?> GetProductsByCategoryId(int categoryId, ProductQueryParams queryParams);
    Task<ProductResponse?> GetProductByIdAsync(int id);
}