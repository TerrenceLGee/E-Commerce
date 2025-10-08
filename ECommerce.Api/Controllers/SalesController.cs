using System.Security.Claims;
using ECommerce.Domain.Interfaces.Services;
using ECommerce.Shared.Dtos.Sales.Request;
using ECommerce.Shared.Dtos.Sales.Response;
using ECommerce.Shared.Dtos.Shared.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class SalesController : ControllerBase
{
    private readonly ISaleService _saleService;

    public SalesController(ISaleService saleService)
    {
        _saleService = saleService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateSale([FromBody] CreateSaleRequest request)
    {
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (customerId is null)
        {
            return Unauthorized();
        }

        var result = await _saleService.CreateSaleAsync(request, customerId);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.FirstOrDefault()?.Message);
        }

        return CreatedAtAction(
            nameof(GetSaleForUserById),
            new {id = result.Value!.Id},
            result.Value);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllSales([FromQuery] SaleQueryParams queryParams)
    {
        var result = await _saleService.GetAllSalesAsync(queryParams);

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
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetSaleById([FromRoute] int id)
    {
        var result = await _saleService.GetSaleByIdAsync(id);

        if (result.IsFailed)
        {
            var errorMessage = result.Errors.FirstOrDefault()?.Message;
            return errorMessage!.Contains("not found")
                ? NotFound(errorMessage)
                : BadRequest(errorMessage);
        }

        return Ok(result.Value);
    }

    [HttpGet("me/sales")]
    [Authorize]
    public async Task<IActionResult> GetSalesByUserId([FromQuery] SaleQueryParams queryParams)
    {
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (customerId is null)
        {
            return Unauthorized();
        }

        var result = await _saleService.GetUserSalesAsync(customerId, queryParams);

        if (result.IsFailed)
        {
            return NotFound(result.Errors.FirstOrDefault()?.Message);
        }

        return Ok(result.Value);
    }

    [HttpGet("me/sales/{id}")]
    [Authorize]
    public async Task<IActionResult> GetSaleForUserById([FromRoute] int id)
    {
        var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (customerId is null)
        {
            return Unauthorized();
        }

        var result = await _saleService.GetUserSaleByIdAsync(customerId, id);

        if (result.IsFailed)
        {
            var errorMessage = result.Errors.FirstOrDefault()?.Message;
            return errorMessage!.Contains("not found")
                ? NotFound(errorMessage)
                : BadRequest(errorMessage);
        }

        return Ok(result.Value);
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateSaleStatus([FromRoute] int id, [FromBody] UpdateSaleStatusRequest request)
    {
        var result = await _saleService.UpdateSaleStatusAsync(id, request.UpdatedStatus);

        if (result.IsFailed)
        {
            var errorMessage = result.Errors.FirstOrDefault()?.Message;
            return errorMessage!.Contains("not found")
                ? NotFound(errorMessage)
                : BadRequest(errorMessage);
        }

        return Ok();
    }

    [HttpPost("{id}/cancel")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AdminCancelSale([FromRoute] int id)
    {
        var result = await _saleService.CancelSaleAsync(id);

        if (result.IsFailed)
        {
            var errorMessage = result.Errors.FirstOrDefault()?.Message;
            return errorMessage!.Contains("not found")
                ? NotFound(errorMessage)
                : BadRequest(errorMessage);
        }

        return Ok();
    }

    [HttpPost("me/sales/cancel/{id}")]
    [Authorize]
    public async Task<IActionResult> UserCancelSale(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null)
        {
            return Unauthorized();
        }

        var result = await _saleService.UserCancelSaleAsync(userId, id);

        if (result.IsFailed)
        {
            var errorMessage = result.Errors.FirstOrDefault()?.Message;
            return errorMessage!.Contains("not found")
                ? NotFound(errorMessage)
                : BadRequest(errorMessage);
        }

        return Ok();
    }

    [HttpPut("{id}/refund")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RefundSale(int id)
    {
        var result = await _saleService.RefundSaleAsync(id);

        if (result.IsFailed)
        {
            var errorMessage = result.Errors.FirstOrDefault()?.Message;
            return errorMessage!.Contains("not found")
                ? NotFound(errorMessage)
                : BadRequest(errorMessage);
        }

        return Ok();
    }
}