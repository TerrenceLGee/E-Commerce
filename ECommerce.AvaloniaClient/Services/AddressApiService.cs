using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using ECommerce.AvaloniaClient.Interfaces.Api;
using ECommerce.Shared.Dtos.Addresses.Request;
using ECommerce.Shared.Dtos.Addresses.Response;
using ECommerce.Shared.Dtos.Shared.Pagination;
using Microsoft.Extensions.Logging;

namespace ECommerce.AvaloniaClient.Services;

public class AddressApiService : IAddressApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AddressApiService> _logger;
    private readonly JsonSerializerOptions _options;

    public AddressApiService(
        IHttpClientFactory httpClientFactory,
        ILogger<AddressApiService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("Client");
        _logger = logger;
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }


    public async Task<AddressResponse?> AddAddressAsync(CreateAddressRequest request)
    {
        try
        {
            var queryString = "api/addresses";
            var response = await _httpClient.PostAsJsonAsync(queryString, request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error adding address. Status: {statusCode}, Reason: {reason}", response.StatusCode,
                    response.ReasonPhrase);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<AddressResponse>(_options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while adding an address");
            return null;
        }
    }

    public async Task<AddressResponse?> UpdateAddressAsync(int addressId, UpdateAddressRequest request)
    {
        try
        {
            var queryString = $"api/addresses/{addressId}";
            var response = await _httpClient.PutAsJsonAsync(queryString, request);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error updating address. Status: {statusCode}, Reason: {reason}", response.StatusCode,
                    response.ReasonPhrase);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<AddressResponse>(_options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while updating the address");
            return null;
        }
    }

    public async Task<AddressResponse?> DeleteAddressAsync(int addressId)
    {
        try
        {
            var queryString = $"api/addresses/{addressId}";
            var response = await _httpClient.DeleteAsync(queryString);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error deleting address. Status: {statusCode}, Reason: {reason}", response.StatusCode,
                    response.ReasonPhrase);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<AddressResponse>(_options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while deleting the address");
            return null;
        }
    }

    public async Task<PagedList<AddressResponse>?> GetAllAddressesAsync(PaginationParams paginationParams)
    {
        try
        {
            var queryString = 
                $"api/addresses?pageNumber={paginationParams.PageNumber}&pageSize={paginationParams.PageSize}";
            var response = await _httpClient.GetAsync(queryString);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error retrieving addresses. Status: {statusCode}, Reason: {reason}",
                    response.StatusCode, response.ReasonPhrase);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<PagedList<AddressResponse>>(_options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while retrieving the addresses");
            return null;
        }
    }

    public async Task<AddressResponse?> GetAddressByIdAsync(int addressId)
    {
        try
        {
            var queryString = $"api/addresses/{addressId}";
            var response = await _httpClient.GetAsync(queryString);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error retrieving address with Id: {id}. Status: {statusCode}, Reason: {reason}",
                    addressId, response.StatusCode, response.ReasonPhrase);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<AddressResponse>(_options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while retrieving address with Id: {id}", addressId);
            return null;
        }
    }
}