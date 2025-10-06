using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using ECommerce.AvaloniaClient.Interfaces.Api;
using ECommerce.Shared.Dtos.Products.Request;
using ECommerce.Shared.Dtos.Products.Response;
using ECommerce.Shared.Dtos.Shared.Pagination;
using Microsoft.Extensions.Logging;

namespace ECommerce.AvaloniaClient.Services;

public class ProductsApiService : IProductsApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProductsApiService> _logger;
    private readonly JsonSerializerOptions _options;

    public ProductsApiService(
        IHttpClientFactory httpClientFactory,
        ILogger<ProductsApiService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("Client");
        _logger = logger;
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }


    public async Task<ProductResponse?> CreateProductAsync(CreateProductRequest request)
    {
        try
        {
            var queryString = "api/products";
            var response = await _httpClient.PostAsJsonAsync(queryString, request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error creating product. Status: {statusCode}, Reason: {reason}", response.StatusCode,
                    response.ReasonPhrase);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<ProductResponse>(_options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while adding the product");
            return null;
        }
    }

    public async Task<ProductResponse?> UpdateProductAsync(int id, UpdateProductRequest request)
    {
        try
        {
            var queryString = $"api/products/{id}";
            var response = await _httpClient.PutAsJsonAsync(queryString, request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error updating product with Id: {id}. Status: {statusCode}, Reason: {reason}", id,
                    response.StatusCode, response.ReasonPhrase);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<ProductResponse>(_options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while updating the product with Id: {id}", id);
            return null;
        }
    }

    public async Task<ProductResponse?> DeleteProductAsync(int id)
    {
        try
        {
            var queryString = $"api/products/{id}";
            var response = await _httpClient.DeleteAsync(queryString);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error deleting product with Id: {id}. Status: {statusCode}, Reason: {reason}", id,
                    response.StatusCode, response.ReasonPhrase);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<ProductResponse>(_options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while deleting the product with Id: {id}", id);
            return null;
        }
    }

    public async Task<PagedList<ProductResponse>?> GetProductsAsync(PaginationParams paginationParams)
    {
        try
        {
            var queryString = 
                $"api/products?pageNumber={paginationParams.PageNumber}&pageSize={paginationParams.PageSize}";
            var response = await _httpClient.GetAsync(queryString);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error retieving products. Status: {statusCode}, Reason: {reason}",
                    response.StatusCode, response.ReasonPhrase);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<PagedList<ProductResponse>>(_options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while retrieving the products");
            return null;
        }
    }

    public async Task<ProductResponse?> GetProductByIdAsync(int id)
    {
        try
        {
            var queryString = $"api/products/{id}";
            var response = await _httpClient.GetAsync(queryString);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error retrieving product with Id: {id}. Status: {statusCode}, Reason: {reason}", id,
                    response.StatusCode, response.ReasonPhrase);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<ProductResponse>(_options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while retrieving the product with Id: {id}", id);
            return null;
        }
    }
}