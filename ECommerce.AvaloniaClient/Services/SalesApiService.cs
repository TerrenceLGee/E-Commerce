using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using ECommerce.AvaloniaClient.Interfaces.Api;
using ECommerce.Shared.Dtos.Sales.Request;
using ECommerce.Shared.Dtos.Sales.Response;
using ECommerce.Shared.Dtos.Shared.Pagination;
using Microsoft.Extensions.Logging;

namespace ECommerce.AvaloniaClient.Services;

public class SalesApiService : ISalesApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SalesApiService> _logger;
    private readonly JsonSerializerOptions _options;

    public SalesApiService(
        IHttpClientFactory httpClientFactory,
        ILogger<SalesApiService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("Client");
        _logger = logger;
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }

    public async Task<SaleResponse?> CreateSaleAsync(CreateSaleRequest saleRequest)
    {
        try
        {
            var queryString = "api/sales";
            var response = await _httpClient.PostAsJsonAsync(queryString, saleRequest);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error creating sale. Status: {statusCode}, Reason: {reason}:", response.StatusCode,
                    response.ReasonPhrase);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<SaleResponse>(_options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "There was an unexpected error creating the sale");
            return null;
        }
    }

    public async Task<bool> UpdateSaleAsync(int id, UpdateSaleStatusRequest updateRequest)
    {
        try
        {
            var queryString = $"api/sales/{id}/status";
            var response = await _httpClient.PutAsJsonAsync(queryString, updateRequest);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error updating sale with Id: {id}. Status: {statusCode}, Reason: {reason}", id,
                    response.StatusCode, response.ReasonPhrase);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while updating sale with Id: {id}", id);
            return false;
        }
    }

    public async Task<bool> RefundSaleAsync(int id)
    {
        try
        {
            var queryString = $"api/sales/{id}/refund";
            var response = await _httpClient.PutAsync(queryString, null);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error refunding sale with Id: {id}. Status: {statusCode}, Reason: {reason}", id,
                    response.StatusCode, response.ReasonPhrase);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while refunding sale with Id: {id}", id);
            return false;
        }
    }

    public async Task<bool> CancelSaleAsync(int id)
    {
        try
        {
            var queryString = $"api/sales/{id}/cancel";
            var response = await _httpClient.PostAsync(queryString, null);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error canceling sale with Id: {id}, Status: {statusCode}, Reason: {reason}", id,
                    response.StatusCode, response.ReasonPhrase);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while canceling sale with Id: {id}", id);
            return false;
        }
    }

    public async Task<bool> UserCancelSaleAsync(int id)
    {
        try
        {
            var queryString = $"api/sales/me/sales/cancel/{id}";
            var response = await _httpClient.PostAsync(queryString, null);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error canceling sale with Id: {id}. Status: {statusCode}, Reason: {reason}", id,
                    response.StatusCode, response.ReasonPhrase);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while canceling sale with Id: {id}", id);
            return false;
        }
    }

    public async Task<PagedList<SaleResponse>?> GetAllSalesAsync(SaleQueryParams queryParams)
    {
        try
        {
            var queryString = 
                $"api/sales?pageNumber={queryParams.PageNumber}&pageSize={queryParams.PageSize}";
            var response = await _httpClient.GetAsync(queryString);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error retrieving all sales. Status: {statusCode}, Reason: {reason}",
                    response.StatusCode, response.ReasonPhrase);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<PagedList<SaleResponse>>(_options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while retrieving all sales");
            return null;
        }
    }

    public async Task<PagedList<SaleResponse>?> GetAllSalesForUserAsync(SaleQueryParams queryParams)
    {
        try
        {
            var queryString = 
                $"api/sales/me/sales?pageNumber={queryParams.PageNumber}&pageSize={queryParams.PageSize}";
            var response = await _httpClient.GetAsync(queryString);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error retrieving all sales. Status: {statusCode}, Reason: {reason}",
                    response.StatusCode, response.ReasonPhrase);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<PagedList<SaleResponse>>(_options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while retrieving all sales");
            return null;
        }
    }

    public async Task<SaleResponse?> GetSaleForUserByIdAsync(int saleId)
    {
        try
        {
            var queryString = $"api/sales/{saleId}";
            var response = await _httpClient.GetAsync(queryString);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error retrieving sale with Id: {id}. Status: {statusCode}, Reason: {reason}", saleId,
                    response.StatusCode, response.ReasonPhrase);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<SaleResponse>(_options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while retrieving sale with Id: {id}", saleId);
            return null;
        }
    }

    public async Task<SaleResponse?> GetSaleByIdAsync(int saleId)
    {
        try
        {
            var queryString = $"api/sales/me/sales/{saleId}";
            var response = await _httpClient.GetAsync(queryString);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error retrieving sale with Id: {id}. Status: {statusCode}, Reason: {reason}", saleId,
                    response.StatusCode, response.ReasonPhrase);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<SaleResponse>(_options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while retrieving sale with Id: {id}", saleId);
            return null;
        }
    }
}