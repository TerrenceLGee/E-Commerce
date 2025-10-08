using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ECommerce.Domain.Interfaces.Services;
using ECommerce.Shared.Dtos.Addresses.Request;
using ECommerce.Shared.Dtos.Shared.Pagination;

namespace ECommerce.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AddressesController : ControllerBase
{
    private readonly IAddressService _addressService;

    public AddressesController(IAddressService addressService)
    {
        _addressService = addressService;
    }
    
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllAddresses([FromQuery] AddressQueryParams queryParams)
    {
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (customerId is null)
        {
            return Unauthorized();
        }

        var result = await _addressService.GetAllAddressesAsync(customerId, queryParams);

        if (result.IsFailed)
        {
            var errorMessage = result.Errors.FirstOrDefault()?.Message;
            return errorMessage!.Contains("not found")
                ? NotFound(errorMessage)
                : BadRequest(errorMessage);
        }

        return Ok(result.Value);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetAddressById([FromRoute] int id)
    {
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (customerId is null)
        {
            return Unauthorized();
        }

        var result = await _addressService.GetAddressByIdAsync(customerId, id);

        if (result.IsFailed)
        {
            var errorMessage = result.Errors.FirstOrDefault()?.Message;
            return errorMessage!.Contains("not found")
                ? NotFound(errorMessage)
                : BadRequest(errorMessage);
        }

        return Ok(result.Value);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddAddress([FromBody] CreateAddressRequest request)
    {
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (customerId is null)
        {
            return Unauthorized();
        }

        var result = await _addressService.AddAddressAsync(customerId, request);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.FirstOrDefault()?.Message);
        }

        return CreatedAtAction(
            nameof(GetAddressById),
            new { id = result.Value.Id },
            result.Value);
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateAddress([FromBody] UpdateAddressRequest request, [FromRoute] int id)
    {
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (customerId is null)
        {
            return Unauthorized();
        }

        var result = await _addressService.UpdateAddressAsync(customerId, id, request);

        if (result.IsFailed)
        {
            var errorMessage = result.Errors.FirstOrDefault()?.Message;
            return errorMessage!.Contains("not found")
                ? NotFound(errorMessage)
                : BadRequest(errorMessage);
        }

        return Ok(result.Value);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteAddress([FromRoute] int id)
    {
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (customerId is null)
        {
            return Unauthorized();
        }

        var result = await _addressService.DeleteAddressAsync(customerId, id);

        if (result.IsFailed)
        {
            var errorMessage = result.Errors.FirstOrDefault()?.Message;
            return errorMessage!.Contains("not found")
                ? NotFound(errorMessage)
                : BadRequest(errorMessage);
        }

        return Ok(result.Value);
    }
}