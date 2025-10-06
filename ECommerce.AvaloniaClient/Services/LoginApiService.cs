using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ECommerce.AvaloniaClient.Interfaces.Api;
using ECommerce.AvaloniaClient.Interfaces.Auth;
using ECommerce.Shared.Dtos.Auth.Request;
using ECommerce.Shared.Dtos.Auth.Response;
using ECommerce.Shared.Dtos.Auth.Errors;

namespace ECommerce.AvaloniaClient.Services;

public class LoginApiService : ILoginApiService
{
    private readonly HttpClient _httpClient;
    private readonly IAuthTokenHolder _tokenHolder;
    private readonly ILogger<LoginApiService> _logger;
    public string? JwtToken { get; private set; }
    
    public LoginApiService(
        IHttpClientFactory httpClientFactory,
        IAuthTokenHolder tokenHolder,
        ILogger<LoginApiService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("Client");
        _tokenHolder = tokenHolder;
        _logger = logger;
    }
    
    public async Task<bool> LoginAsync(LoginRequest request)
    {
        try
        {
            var queryString = "api/auth/login";
            var response = await _httpClient.PostAsJsonAsync(queryString, request);

            if (!response.IsSuccessStatusCode)
            {
                _tokenHolder.SetToken(null);
                _logger.LogError("Error logging in. Status: {statusCode}, Reason: {reason}", response.StatusCode,
                    response.ReasonPhrase);

                return false;
            }

            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();

            if (authResponse?.Token is null)
            {
                _tokenHolder.SetToken(null);
                _logger.LogError("Login token invalid");
                return false;
            }

            _tokenHolder.SetToken(authResponse.Token);
            JwtToken = authResponse.Token;
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while attempting to login");
            return false;
        }
    }

    public async Task<bool> RegisterAsync(RegisterRequest request)
    {
        try
        {
            var queryString = "api/auth/register";
            var response = await _httpClient.PostAsJsonAsync(queryString, request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                try
                {
                    var errors =
                        JsonSerializer.Deserialize<List<IdentityError>>(errorContent,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (errors is not null && errors.Any())
                    {
                        foreach (var error in errors)
                        {
                            _logger.LogError("Error: {errorDescription}", error.Description);
                        }
                    }
                }
                catch (JsonException ex)
                {
                    _logger.LogError("API Error: {errorContent}\n{errorMessage}", errorContent, ex.Message);
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while attempting to register new user");
            return false;
        }
    }

    public ClaimsPrincipal? GetPrincipalFromToken(string token)
    {
        try
        {
            if (string.IsNullOrEmpty(JwtToken))
            {
                return new ClaimsPrincipal();
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var identity = new ClaimsIdentity(jwtToken.Claims, "jwt");
            return new ClaimsPrincipal(identity);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "An unexpected error occurred while getting the principal");
            return null;
        }
    }
}