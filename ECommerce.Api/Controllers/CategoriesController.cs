using ECommerce.Domain.Interfaces.Services;
using ECommerce.Shared.Dtos.Categories.Request;
using ECommerce.Shared.Dtos.Categories.Response;
using ECommerce.Shared.Dtos.Shared.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllCategories([FromQuery] PaginationParams paginationParams)
    {
        var result = await _categoryService.GetAllCategoriesAsync(paginationParams);

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
    [AllowAnonymous]
    public async Task<IActionResult> GetCategoryById([FromRoute] int id)
    {
        var result = await _categoryService.GetCategoryByIdAsync(id);

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
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request)
    {
        var result = await _categoryService.CreateCategoryAsync(request);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.FirstOrDefault()?.Message);
        }

        return CreatedAtAction(
            nameof(GetCategoryById),
            new {id = result.Value!.Id},
            result.Value);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CategoryResponse>> UpdateCategory([FromRoute] int id, [FromBody] UpdateCategoryRequest request)
    {
        var result = await _categoryService.UpdateCategoryAsync(id, request);

        if (result.IsFailed)
        {
            var errorMessage = result.Errors.FirstOrDefault()?.Message;
            return errorMessage!.Contains("not found")
                ? NotFound(errorMessage)
                : BadRequest(errorMessage);
        }

        return Ok(result.Value);
    }

    [HttpDelete]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CategoryResponse>> DeleteCategory([FromRoute] int id)
    {
        var result = await _categoryService.DeleteCategoryAsync(id);

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