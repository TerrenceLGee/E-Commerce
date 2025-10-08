using ECommerce.Domain.Interfaces.Services;
using ECommerce.Shared.Dtos.Products.Request;
using ECommerce.Shared.Dtos.Products.Response;
using ECommerce.Shared.Dtos.Shared.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace ECommerce.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllProducts([FromQuery] ProductQueryParams queryParams)
    {
        var result = await _productService.GetAllProductsAsync(queryParams);

        if (result.IsFailed)
        {
            var errorMessage = result.Errors.FirstOrDefault()?.Message;
            return errorMessage!.Contains("not found")
                ? NotFound(errorMessage)
                : BadRequest(errorMessage);
        }

        return Ok(result.Value);
    }

    [HttpGet("/categories/{categoryId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProductsByCategoryId([FromRoute] int categoryId,
        [FromQuery] ProductQueryParams queryParams)
    {
        var result = await _productService.GetAllProductsByCategoryIdAsync(categoryId, queryParams);

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
    public async Task<IActionResult> GetProductById([FromRoute] int id)
    {
        var result = await _productService.GetProductByIdAsync(id);

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
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
    {
        var result = await _productService.CreateProductAsync(request);

        if (result.IsFailed)
        {
            return BadRequest(result.Errors.FirstOrDefault()?.Message);
        }

        return CreatedAtAction(
            nameof(GetProductById),
            new { id = result.Value.Id },
            result.Value);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductResponse>> UpdateProduct([FromRoute] int id,
        [FromBody] UpdateProductRequest request)
    {
        var result = await _productService.UpdateProductAsync(id, request);

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
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductResponse>> DeleteProduct(int id)
    {
        var result = await _productService.DeleteProductAsync(id);

        if (result.IsFailed)
        {
            return NotFound(result.Errors.FirstOrDefault()?.Message);
        }

        return Ok(result.Value);
    }
}