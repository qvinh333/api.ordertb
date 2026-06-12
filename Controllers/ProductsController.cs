using API.Sale.DTOs;
using API.Sale.DTOs.Product;
using API.Sale.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Sale.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN,SELLER")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly CurrentUserService _currentUserService;

    public ProductsController(IProductService productService, CurrentUserService currentUserService)
    {
        _productService = productService;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> GetProducts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null)
    {
        var userId = _currentUserService.GetUserId();
        var (products, total) = await _productService.GetProductsAsync(userId, page, pageSize, search);

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Success",
            Data = new
            {
                products,
                total,
                page,
                pageSize,
                totalPages = (total + pageSize - 1) / pageSize
            }
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ProductResponse>>> GetProductById(long id)
    {
        var userId = _currentUserService.GetUserId();
        var product = await _productService.GetProductByIdAsync(id, userId);
        if (product == null)
        {
            return NotFound(new ApiResponse<ProductResponse>
            {
                Success = false,
                Message = "Product not found"
            });
        }

        var response = new ProductResponse
        {
            Id = product.Id,
            ProductCode = product.ProductCode,
            Name = product.Name,
            Specification = product.Specification,
            Unit = product.Unit,
            DefaultSellingPrice = product.DefaultSellingPrice,
            Note = product.Note,
            CreatedBy = product.CreatedBy,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };

        return Ok(new ApiResponse<ProductResponse>
        {
            Success = true,
            Message = "Success",
            Data = response
        });
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ProductResponse>>> CreateProduct([FromBody] CreateProductRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ProductCode) || string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(new ApiResponse<ProductResponse>
            {
                Success = false,
                Message = "ProductCode and Name are required"
            });
        }

        try
        {
            var userId = _currentUserService.GetUserId();
            var product = await _productService.CreateProductAsync(request, userId);

            var response = new ProductResponse
            {
                Id = product.Id,
                ProductCode = product.ProductCode,
                Name = product.Name,
                Specification = product.Specification,
                Unit = product.Unit,
                DefaultSellingPrice = product.DefaultSellingPrice,
                Note = product.Note,
                CreatedBy = product.CreatedBy,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };

            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, new ApiResponse<ProductResponse>
            {
                Success = true,
                Message = "Product created successfully",
                Data = response
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponse<ProductResponse>
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<ProductResponse>>> UpdateProduct(long id, [FromBody] UpdateProductRequest request)
    {
        try
        {
            var userId = _currentUserService.GetUserId();
            var product = await _productService.UpdateProductAsync(id, request, userId);

            var response = new ProductResponse
            {
                Id = product.Id,
                ProductCode = product.ProductCode,
                Name = product.Name,
                Specification = product.Specification,
                Unit = product.Unit,
                DefaultSellingPrice = product.DefaultSellingPrice,
                Note = product.Note,
                CreatedBy = product.CreatedBy,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt
            };

            return Ok(new ApiResponse<ProductResponse>
            {
                Success = true,
                Message = "Product updated successfully",
                Data = response
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponse<ProductResponse>
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> DeleteProduct(long id)
    {
        var userId = _currentUserService.GetUserId();
        var result = await _productService.DeleteProductAsync(id, userId);
        if (!result)
        {
            return NotFound(new ApiResponse
            {
                Success = false,
                Message = "Product not found"
            });
        }

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Product deleted successfully"
        });
    }
}

