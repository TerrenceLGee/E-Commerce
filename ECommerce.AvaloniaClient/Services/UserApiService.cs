using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using ECommerce.AvaloniaClient.Interfaces.Api;
using ECommerce.Shared.Dtos.Addresses.Response;
using ECommerce.Shared.Dtos.Auth.Response;
using ECommerce.Shared.Dtos.Shared.Pagination;
using Microsoft.Extensions.Logging;

namespace ECommerce.AvaloniaClient.Services;

public class UserApiService : IUserApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UserApiService> _logger;
    private readonly JsonSerializerOptions _options;

    public UserApiService(
        IHttpClientFactory httpClientFactory,
        ILogger<UserApiService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("Client");
        _logger = logger;
        _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    }
    
    public async Task<PagedList<UserResponse>?> GetAllUsersAsync(UserQueryParams queryParams)
    {
        try
        {
            var queryString = $"api/users?pageNumber={queryParams.PageNumber}&pageSize={queryParams.PageSize}";
            var response = await _httpClient.GetAsync(queryString);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error retrieving all users. Status: {statusCode}, Reason: {reason}",
                    response.StatusCode, response.ReasonPhrase);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<PagedList<UserResponse>>(_options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while retrieving all users");
            return null;
        }
    }

    public async Task<UserResponse?> GetUserByIdAsync(string userId)
    {
        try
        {
            var queryString = $"api/users/{userId}";
            var response = await _httpClient.GetAsync(queryString);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Error retrieving user with Id: {id}. Status: {statusCode}, Reason: {reason}", userId,
                    response.StatusCode, response.ReasonPhrase);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<UserResponse>(_options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while retrieving user with Id: {id}", userId);
            return null;
        }
    }

    public async Task<PagedList<AddressResponse>?> GetUserAddressesByIdAsync(string userId, AddressQueryParams queryParams)
    {
        try
        {
            var queryString = $"api/users/{userId}/addresses?pageNumber={queryParams.PageNumber}&pageSize={queryParams.PageSize}";
            var response = await _httpClient.GetAsync(queryString);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "Error retrieving addresses for user with Id: {id}. Status: {statusCode}, Reason: {reason}", userId,
                    response.StatusCode, response.ReasonPhrase);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<PagedList<AddressResponse>>(_options);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while retrieving addresses for user with Id: {id}",
                userId);
            return null;
        }
    }
}