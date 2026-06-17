using API.Sale.DTOs;
using API.Sale.DTOs.Order;
using API.Sale.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Sale.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN,SELLER")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;
    private readonly CurrentUserService _currentUserService;

    public OrdersController(IOrderService orderService, CurrentUserService currentUserService)
    {
        _orderService = orderService;
        _currentUserService = currentUserService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> GetOrders(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? customerName = null,
        [FromQuery] string? productName = null,
        [FromQuery] string? status = null,
        [FromQuery] string? paymentStatus = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        var userId = _currentUserService.GetUserId();
        var (orders, total) = await _orderService.GetOrdersAsync(userId, page, pageSize, customerName, productName, status, paymentStatus, fromDate, toDate);

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Success",
            Data = new
            {
                orders,
                total,
                page,
                pageSize,
                totalPages = (total + pageSize - 1) / pageSize
            }
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<OrderResponse>>> GetOrderById(long id)
    {
        var userId = _currentUserService.GetUserId();
        var order = await _orderService.GetOrderByIdAsync(id, userId);
        if (order == null)
        {
            return NotFound(new ApiResponse<OrderResponse>
            {
                Success = false,
                Message = "Order not found"
            });
        }

        var response = new OrderResponse
        {
            Id = order.Id,
            OrderCode = order.OrderCode,
            OrderDate = order.OrderDate,
            CustomerName = order.CustomerName,
            ProductName = order.ProductName,
            Specification = order.Specification,
            Quantity = order.Quantity,
            SellingPrice = order.SellingPrice,
            AmountSellingPrice = order.AmountSellingPrice,
            Status = order.Status.ToString(),
            PaymentStatus = order.PaymentStatus.ToString(),
            YuanPrice = order.YuanPrice,
            ImportPrice = order.ImportPrice,
            Supplier = order.Supplier,
            WarehousePayment = order.WarehousePayment,
            ShippingWeightFee = order.ShippingWeightFee,
            ShippingPaymentDate = order.ShippingPaymentDate,
            RefundAmount = order.RefundAmount,
            RefundStatus = order.RefundStatus,
            Note = order.Note,
            CreatedBy = order.CreatedBy,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt
        };

        return Ok(new ApiResponse<OrderResponse>
        {
            Success = true,
            Message = "Success",
            Data = response
        });
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<OrderResponse>>> CreateOrder([FromBody] CreateOrderRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.OrderCode) || string.IsNullOrWhiteSpace(request.CustomerName) || string.IsNullOrWhiteSpace(request.ProductName))
        {
            return BadRequest(new ApiResponse<OrderResponse>
            {
                Success = false,
                Message = "OrderCode, CustomerName, and ProductName are required"
            });
        }

        if (string.IsNullOrWhiteSpace(request.Status) || string.IsNullOrWhiteSpace(request.PaymentStatus))
        {
            return BadRequest(new ApiResponse<OrderResponse>
            {
                Success = false,
                Message = "Status and PaymentStatus are required"
            });
        }

        try
        {
            var userId = _currentUserService.GetUserId();
            var order = await _orderService.CreateOrderAsync(request, userId);

            var response = new OrderResponse
            {
                Id = order.Id,
                OrderCode = order.OrderCode,
                OrderDate = order.OrderDate,
                CustomerName = order.CustomerName,
                ProductName = order.ProductName,
                Specification = order.Specification,
                Quantity = order.Quantity,
                SellingPrice = order.SellingPrice,
                AmountSellingPrice = order.AmountSellingPrice,
                Status = order.Status.ToString(),
                PaymentStatus = order.PaymentStatus.ToString(),
                YuanPrice = order.YuanPrice,
                ImportPrice = order.ImportPrice,
                Supplier = order.Supplier,
                WarehousePayment = order.WarehousePayment,
                ShippingWeightFee = order.ShippingWeightFee,
                ShippingPaymentDate = order.ShippingPaymentDate,
                RefundAmount = order.RefundAmount,
                RefundStatus = order.RefundStatus,
                Note = order.Note,
                CreatedBy = order.CreatedBy,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt
            };

            return CreatedAtAction(nameof(GetOrderById), new { id = order.Id }, new ApiResponse<OrderResponse>
            {
                Success = true,
                Message = "Order created successfully",
                Data = response
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponse<OrderResponse>
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<OrderResponse>>> UpdateOrder(long id, [FromBody] UpdateOrderRequest request)
    {
        try
        {
            var userId = _currentUserService.GetUserId();
            var order = await _orderService.UpdateOrderAsync(id, request, userId);

            var response = new OrderResponse
            {
                Id = order.Id,
                OrderCode = order.OrderCode,
                OrderDate = order.OrderDate,
                CustomerName = order.CustomerName,
                ProductName = order.ProductName,
                Specification = order.Specification,
                Quantity = order.Quantity,
                SellingPrice = order.SellingPrice,
                AmountSellingPrice = order.AmountSellingPrice,
                Status = order.Status.ToString(),
                PaymentStatus = order.PaymentStatus.ToString(),
                YuanPrice = order.YuanPrice,
                ImportPrice = order.ImportPrice,
                Supplier = order.Supplier,
                WarehousePayment = order.WarehousePayment,
                ShippingWeightFee = order.ShippingWeightFee,
                ShippingPaymentDate = order.ShippingPaymentDate,
                RefundAmount = order.RefundAmount,
                RefundStatus = order.RefundStatus,
                Note = order.Note,
                CreatedBy = order.CreatedBy,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt
            };

            return Ok(new ApiResponse<OrderResponse>
            {
                Success = true,
                Message = "Order updated successfully",
                Data = response
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponse<OrderResponse>
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    [HttpPatch("{id}/status")]
    public async Task<ActionResult<ApiResponse<OrderResponse>>> UpdateOrderStatus(long id, [FromBody] UpdateOrderStatusRequest request)
    {
        if (string.IsNullOrEmpty(request.Status))
        {
            return BadRequest(new ApiResponse<OrderResponse>
            {
                Success = false,
                Message = "Status is required"
            });
        }

        try
        {
            var userId = _currentUserService.GetUserId();
            var order = await _orderService.UpdateOrderStatusAsync(id, request.Status, userId);

            var response = new OrderResponse
            {
                Id = order.Id,
                OrderCode = order.OrderCode,
                OrderDate = order.OrderDate,
                CustomerName = order.CustomerName,
                ProductName = order.ProductName,
                Specification = order.Specification,
                Quantity = order.Quantity,
                SellingPrice = order.SellingPrice,
                AmountSellingPrice = order.AmountSellingPrice,
                Status = order.Status.ToString(),
                PaymentStatus = order.PaymentStatus.ToString(),
                YuanPrice = order.YuanPrice,
                ImportPrice = order.ImportPrice,
                Supplier = order.Supplier,
                WarehousePayment = order.WarehousePayment,
                ShippingWeightFee = order.ShippingWeightFee,
                ShippingPaymentDate = order.ShippingPaymentDate,
                RefundAmount = order.RefundAmount,
                RefundStatus = order.RefundStatus,
                Note = order.Note,
                CreatedBy = order.CreatedBy,
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt
            };

            return Ok(new ApiResponse<OrderResponse>
            {
                Success = true,
                Message = "Order status updated successfully",
                Data = response
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponse<OrderResponse>
            {
                Success = false,
                Message = ex.Message
            });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse>> DeleteOrder(long id)
    {
        var userId = _currentUserService.GetUserId();
        var result = await _orderService.DeleteOrderAsync(id, userId);
        if (!result)
        {
            return NotFound(new ApiResponse
            {
                Success = false,
                Message = "Order not found"
            });
        }

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Order deleted successfully"
        });
    }
}

