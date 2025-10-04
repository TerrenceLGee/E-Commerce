using ECommerce.Domain.Interfaces.Services;
using ECommerce.Domain.Models;
using ECommerce.Infrastructure.Identity;
using ECommerce.Shared.Dtos.Auth.Request;
using ECommerce.Shared.Dtos.Auth.Response;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var newUser = new ApplicationUser
        {
            Email = request.Email,
            UserName = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            DateOfBirth = request.BirthDate,
            Addresses = new List<Address>()
        };

        var userAddress = new Address
        {
            StreetNumber = request.StreetNumber,
            StreetName = request.StreetName,
            City = request.City,
            State = request.State,
            Country = request.Country,
            ZipCode = request.ZipCode,
            AddressType = request.AddressType
        };

        newUser.Addresses.Add(userAddress);

        var result = await _userManager.CreateAsync(newUser, request.Password);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        await _userManager.AddToRoleAsync(newUser, "Customer");

        var roles = await _userManager.GetRolesAsync(newUser);
        var token = await _tokenService.CreateTokenAsync(newUser.Id, newUser.Email, roles);

        return Ok(new AuthResponse
        {
            Email = newUser.Email,
            Token = token
        });
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return Unauthorized("Invalid email or password");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = await _tokenService.CreateTokenAsync(user.Id, user.Email!, roles);

        return Ok(new AuthResponse
        {
            Email = user.Email!,
            Token = token
        });
    }
}
    
