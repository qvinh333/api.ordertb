using API.Sale.DTOs;
using API.Sale.DTOs.Customer;
using API.Sale.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Sale.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN,SELLER")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;
    private readonly CurrentUserService _currentUserService;

    public CustomersController(ICustomerService customerService, CurrentUserService currentUserService)
    {
        _customerService = customerService;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> GetCustomers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null)
    {
        var userId = _currentUserService.GetUserId();
        var (customers, total) = await _customerService.GetCustomersAsync(userId, page, pageSize, search);

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Success",
            Data = new
            {
                customers,
                total,
                page,
                pageSize,
                totalPages = (total + pageSize - 1) / pageSize
            }
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<CustomerResponse>>> GetCustomerById(long id)
    {
        var userId = _currentUserService.GetUserId();
        var customer = await _customerService.GetCustomerByIdAsync(id, userId);
        if (customer == null)
        {
            return NotFound(new ApiResponse<CustomerResponse>
            {
                Success = false,
                Message = "Customer not found"
            });
        }

        var response = new CustomerResponse
        {
            Id = customer.Id,
            CustomerCode = customer.CustomerCode,
            FullName = customer.FullName,
            Phone = customer.Phone,
            Email = customer.Email,
            Address = customer.Address,
            Note = customer.Note,
            CreatedBy = customer.CreatedBy,
            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt
        };

        return Ok(new ApiResponse<CustomerResponse>
        {
            Success = true,
            Message = "Success",
            Data = response
        });
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<CustomerResponse>>> CreateCustomer([FromBody] CreateCustomerRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.CustomerCode) || string.IsNullOrWhiteSpace(request.FullName))
        {
            return BadRequest(new ApiResponse<CustomerResponse>
            {
                Success = false,
                Message = "CustomerCode and FullName are required"
            });
        }

        try
        {
            var userId = _currentUserService.GetUserId();
            var customer = await _customerService.CreateCustomerAsync(request, userId);

            var response = new CustomerResponse
            {
                Id = customer.Id,
                CustomerCode = customer.CustomerCode,
                FullName = customer.FullName,
                Phone = customer.Phone,
                Email = customer.Email,
                Address = customer.Address,
                Note = customer.Note,
                CreatedBy = customer.CreatedBy,
                CreatedAt = customer.CreatedAt,
                UpdatedAt = customer.UpdatedAt
            };

            return CreatedAtAction(nameof(GetCustomerById), new { id = customer.Id }, new ApiResponse<CustomerResponse>
            {
                Success = true,
                Message = "Customer created successfully",
                Data = response
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponse<CustomerResponse>
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<CustomerResponse>>> UpdateCustomer(long id, [FromBody] UpdateCustomerRequest request)
    {
        try
        {
            var userId = _currentUserService.GetUserId();
            var customer = await _customerService.UpdateCustomerAsync(id, request, userId);

            var response = new CustomerResponse
            {
                Id = customer.Id,
                CustomerCode = customer.CustomerCode,
                FullName = customer.FullName,
                Phone = customer.Phone,
                Email = customer.Email,
                Address = customer.Address,
                Note = customer.Note,
                CreatedBy = customer.CreatedBy,
                CreatedAt = customer.CreatedAt,
                UpdatedAt = customer.UpdatedAt
            };

            return Ok(new ApiResponse<CustomerResponse>
            {
                Success = true,
                Message = "Customer updated successfully",
                Data = response
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponse<CustomerResponse>
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> DeleteCustomer(long id)
    {
        var userId = _currentUserService.GetUserId();
        var result = await _customerService.DeleteCustomerAsync(id, userId);
        if (!result)
        {
            return NotFound(new ApiResponse
            {
                Success = false,
                Message = "Customer not found"
            });
        }

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Customer deleted successfully"
        });
    }
}

