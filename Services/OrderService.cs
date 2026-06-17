using API.Sale.Data;
using API.Sale.DTOs.Order;
using API.Sale.Models;
using API.Sale.Models.Enums;

namespace API.Sale.Services;

public interface IOrderService
{
    Task<(List<OrderResponse> orders, int total)> GetOrdersAsync(
        long currentUserId,
        int page = 1, 
        int pageSize = 10, 
        string? customerName = null, 
        string? productName = null,
        string? status = null,
        string? paymentStatus = null,
        DateTime? fromDate = null,
        DateTime? toDate = null);
    Task<Order?> GetOrderByIdAsync(long id, long currentUserId);
    Task<Order> CreateOrderAsync(CreateOrderRequest request, long userId);
    Task<Order> UpdateOrderAsync(long id, UpdateOrderRequest request, long currentUserId);
    Task<Order> UpdateOrderStatusAsync(long id, string status, long currentUserId);
    Task<bool> DeleteOrderAsync(long id, long currentUserId);
}

public class OrderService : IOrderService
{
    private readonly AppDbContext _context;

    private static OrderStatus ParseOrderStatus(string? value, string fieldName = "Status")
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"{fieldName} is required");
        }

        if (!Enum.TryParse<OrderStatus>(value.Trim(), true, out var parsed))
        {
            throw new InvalidOperationException($"Invalid {fieldName}");
        }

        return parsed;
    }

    private static PaymentStatus ParsePaymentStatus(string? value, string fieldName = "PaymentStatus")
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidOperationException($"{fieldName} is required");
        }

        if (!Enum.TryParse<PaymentStatus>(value.Trim(), true, out var parsed))
        {
            throw new InvalidOperationException($"Invalid {fieldName}");
        }

        return parsed;
    }

    private static DateTime NormalizeToUtc(DateTime value)
    {
        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
    }

    private static DateTime? NormalizeToUtc(DateTime? value)
    {
        return value.HasValue ? NormalizeToUtc(value.Value) : null;
    }

    private static IQueryable<Order> ApplyUserScope(IQueryable<Order> query, long currentUserId)
    {
        return query.Where(o => o.CreatedBy == currentUserId);
    }

    public OrderService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(List<OrderResponse> orders, int total)> GetOrdersAsync(
        long currentUserId,
        int page = 1, 
        int pageSize = 10, 
        string? customerName = null, 
        string? productName = null,
        string? status = null,
        string? paymentStatus = null,
        DateTime? fromDate = null,
        DateTime? toDate = null)
    {
        var query = ApplyUserScope(_context.Orders.Where(o => !o.Deleted), currentUserId).AsQueryable();

        if (!string.IsNullOrEmpty(customerName))
        {
            query = query.Where(o => o.CustomerName.Contains(customerName));
        }

        if (!string.IsNullOrEmpty(productName))
        {
            query = query.Where(o => o.ProductName.Contains(productName));
        }

        if (!string.IsNullOrEmpty(status))
        {
            if (Enum.TryParse<OrderStatus>(status, true, out var orderStatus))
            {
                query = query.Where(o => o.Status == orderStatus);
            }
        }

        if (!string.IsNullOrEmpty(paymentStatus))
        {
            if (Enum.TryParse<PaymentStatus>(paymentStatus, true, out var parsedPaymentStatus))
            {
                query = query.Where(o => o.PaymentStatus == parsedPaymentStatus);
            }
        }

        if (fromDate.HasValue)
        {
            var utcFromDate = DateTime.SpecifyKind(fromDate.Value.Date, DateTimeKind.Utc);
            query = query.Where(o => o.OrderDate >= utcFromDate);
        }

        if (toDate.HasValue)
        {
            var utcToDateExclusive = DateTime.SpecifyKind(toDate.Value.Date.AddDays(1), DateTimeKind.Utc);
            query = query.Where(o => o.OrderDate < utcToDateExclusive);
        }

        var total = query.Count();
        var orders = query
            .OrderByDescending(o => o.OrderDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(o => MapToResponse(o))
            .ToList();

        return await Task.FromResult((orders, total));
    }

    public async Task<Order?> GetOrderByIdAsync(long id, long currentUserId)
    {
        var query = ApplyUserScope(_context.Orders.Where(o => !o.Deleted), currentUserId);
        return await Task.FromResult(query.FirstOrDefault(o => o.Id == id));
    }

    public async Task<Order> CreateOrderAsync(CreateOrderRequest request, long userId)
    {
        if (_context.Orders.Any(o => o.OrderCode == request.OrderCode))
        {
            throw new InvalidOperationException("Order code already exists");
        }

        var order = new Order
        {
            OrderCode = request.OrderCode,
            OrderDate = NormalizeToUtc(request.OrderDate),
            CustomerName = request.CustomerName,
            ProductName = request.ProductName,
            Specification = request.Specification,
            Quantity = request.Quantity,
            SellingPrice = request.SellingPrice,
            AmountSellingPrice = request.AmountSellingPrice,
            Status = ParseOrderStatus(request.Status),
            PaymentStatus = ParsePaymentStatus(request.PaymentStatus),
            YuanPrice = request.YuanPrice,
            ImportPrice = request.ImportPrice,
            Supplier = request.Supplier,
            WarehousePayment = request.WarehousePayment,
            ShippingWeightFee = request.ShippingWeightFee,
            ShippingPaymentDate = NormalizeToUtc(request.ShippingPaymentDate),
            RefundAmount = request.RefundAmount,
            RefundStatus = request.RefundStatus,
            Note = request.Note,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Deleted = false
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return order;
    }

    public async Task<Order> UpdateOrderAsync(long id, UpdateOrderRequest request, long currentUserId)
    {
        var query = ApplyUserScope(_context.Orders.Where(o => !o.Deleted), currentUserId);
        var order = query.FirstOrDefault(o => o.Id == id);
        if (order == null)
        {
            throw new InvalidOperationException("Order not found");
        }

        if (!string.IsNullOrEmpty(request.OrderCode) && request.OrderCode != order.OrderCode)
        {
            if (_context.Orders.Any(o => o.OrderCode == request.OrderCode && !o.Deleted))
            {
                throw new InvalidOperationException("Order code already exists");
            }
            order.OrderCode = request.OrderCode;
        }

        if (request.OrderDate.HasValue) order.OrderDate = NormalizeToUtc(request.OrderDate.Value);
        if (!string.IsNullOrEmpty(request.CustomerName)) order.CustomerName = request.CustomerName;
        if (!string.IsNullOrEmpty(request.ProductName)) order.ProductName = request.ProductName;
        if (request.Specification != null) order.Specification = request.Specification;
        if (request.Quantity.HasValue) order.Quantity = request.Quantity.Value;
        if (request.SellingPrice.HasValue) order.SellingPrice = request.SellingPrice.Value;
        if (request.AmountSellingPrice.HasValue) order.AmountSellingPrice = request.AmountSellingPrice.Value;
        if (!string.IsNullOrEmpty(request.PaymentStatus))
        {
            order.PaymentStatus = ParsePaymentStatus(request.PaymentStatus);
        }
        if (request.YuanPrice.HasValue) order.YuanPrice = request.YuanPrice;
        if (request.ImportPrice.HasValue) order.ImportPrice = request.ImportPrice;
        if (!string.IsNullOrEmpty(request.Supplier)) order.Supplier = request.Supplier;
        if (request.WarehousePayment.HasValue) order.WarehousePayment = request.WarehousePayment;
        if (request.ShippingWeightFee.HasValue) order.ShippingWeightFee = request.ShippingWeightFee;
        if (request.ShippingPaymentDate.HasValue) order.ShippingPaymentDate = NormalizeToUtc(request.ShippingPaymentDate);
        if (request.RefundAmount.HasValue) order.RefundAmount = request.RefundAmount;
        if (!string.IsNullOrEmpty(request.RefundStatus)) order.RefundStatus = request.RefundStatus;
        if (request.Note != null) order.Note = request.Note;

        order.UpdatedAt = DateTime.UtcNow;
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();

        return order;
    }

    public async Task<Order> UpdateOrderStatusAsync(long id, string status, long currentUserId)
    {
        var query = ApplyUserScope(_context.Orders.Where(o => !o.Deleted), currentUserId);
        var order = query.FirstOrDefault(o => o.Id == id);
        if (order == null)
        {
            throw new InvalidOperationException("Order not found");
        }

        var newStatus = ParseOrderStatus(status);
        order.Status = newStatus;
        order.UpdatedAt = DateTime.UtcNow;

        _context.Orders.Update(order);
        await _context.SaveChangesAsync();

        return order;
    }

    public async Task<bool> DeleteOrderAsync(long id, long currentUserId)
    {
        var query = ApplyUserScope(_context.Orders.Where(o => !o.Deleted), currentUserId);
        var order = query.FirstOrDefault(o => o.Id == id);
        if (order == null)
        {
            return false;
        }

        order.Deleted = true;
        order.Status = OrderStatus.DELETED;
        order.UpdatedAt = DateTime.UtcNow;

        _context.Orders.Update(order);
        await _context.SaveChangesAsync();

        return true;
    }

    private static OrderResponse MapToResponse(Order order)
    {
        return new OrderResponse
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
    }
}

