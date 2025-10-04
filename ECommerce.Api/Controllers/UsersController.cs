using ECommerce.Domain.Interfaces.Services;
using ECommerce.Shared.Dtos.Shared.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllUsers([FromQuery] PaginationParams paginationParams)
    {
        var result = await _userService.GetAllUsersAsync(paginationParams);

        if (result.IsFailed)
        {
            var errorMessage = result.Errors.FirstOrDefault()?.Message;
            return errorMessage!.Contains("not found")
                ? NotFound(errorMessage)
                : BadRequest(errorMessage);
        }

        return Ok(result.Value);
    }

    [HttpGet("{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetUserById([FromRoute] string userId)
    {
        var result = await _userService.GetUserByIdAsync(userId);

        if (result.IsFailed)
        {
            var errorMessage = result.Errors.FirstOrDefault()?.Message;
            return errorMessage!.Contains("not found")
                ? NotFound(errorMessage)
                : BadRequest(errorMessage);
        }

        return Ok(result.Value);
    }

    [HttpGet("{userId}/addresses")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAddressesByUserId([FromRoute] string userId,
        [FromQuery] PaginationParams paginationParams)
    {
        var result = await _userService.GetUserAddressesByIdAsync(userId, paginationParams);

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