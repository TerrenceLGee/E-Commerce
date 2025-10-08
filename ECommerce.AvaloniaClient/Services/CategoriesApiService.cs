using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using ECommerce.AvaloniaClient.Interfaces.Api;
using ECommerce.Shared.Dtos.Categories.Request;
using ECommerce.Shared.Dtos.Categories.Response;
using ECommerce.Shared.Dtos.Shared.Pagination;
using Microsoft.Extensions.Logging;

namespace ECommerce.AvaloniaClient.Services;

public class CategoriesApiService : ICategoriesApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CategoriesApiService> _logger;
    private readonly JsonSerializerOptions _options;

    public CategoriesApiService(
        IHttpClientFactory httpClientFactory,
        ILogger<CategoriesApiService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("Client");
        _logger = logger;
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    public async Task<CategoryResponse?> CreateCategoryAsync(CreateCategoryRequest request)
    {
        try
        {
            var queryString = "api/categories";
            var response = await _httpClient.PostAsJsonAsync(queryString, request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error creating category. Status: {statusCode}, Reason: {reason}", response.StatusCode,
                    response.ReasonPhrase);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<CategoryResponse>(_options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while creating the category");
            return null;
        }
    }

    public async Task<CategoryResponse?> UpdateCategoryAsync(int id, UpdateCategoryRequest request)
    {
        try
        {
            var queryString = $"api/categories/{id}";
            var response = await _httpClient.PutAsJsonAsync(queryString, request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error updating category with Id: {id} Status: {statusCode}, Reason: {reason}", id, response.StatusCode, response.ReasonPhrase);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<CategoryResponse>(_options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while updating category with Id: {id}", id);
            return null;
        }
    }

    public async Task<CategoryResponse?> DeleteCategoryAsync(int id)
    {
        try
        {
            var queryString = $"api/categories/{id}";
            var response = await _httpClient.DeleteAsync(queryString);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error deleting category with Id: {id}. Status: {statusCode}, Reason: {reason}", id,
                    response.StatusCode, response.ReasonPhrase);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<CategoryResponse>(_options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while deleting the category with Id: {id}", id);
            return null;
        }
    }

    public async Task<PagedList<CategoryResponse>?> GetCategoriesAsync(CategoryQueryParams queryParams)
    {
        try
        {
            var queryString = 
                $"api/categories?pageNumber={queryParams.PageNumber}&pageSize={queryParams.PageSize}";
            var response = await _httpClient.GetAsync(queryString);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error retrieving categories. Status: {statusCode}, Reason: {reason}",
                    response.StatusCode, response.ReasonPhrase);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<PagedList<CategoryResponse>>(_options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while retrieving the categories");
            return null;
        }
    }

    public async Task<CategoryResponse?> GetCategoryByIdAsync(int id)
    {
        try
        {
            var queryString = $"api/categories/{id}";
            var response = await _httpClient.GetAsync(queryString);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error retrieving category with Id: {id}. Status: {statusCode}, Reason: {reason}", id,
                    response.StatusCode, response.ReasonPhrase);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<CategoryResponse>(_options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while retrieving category with Id: {id}", id);
            return null;
        }
    }
}