using API.Sale.Data;
using API.Sale.DTOs.Dashboard;
using API.Sale.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace API.Sale.Services;

public interface IDashboardService
{
    Task<OrderStatusStatisticsResponse> GetOrderStatusStatisticsAsync(long currentUserId);
    Task<RevenueResponse> GetRevenueAsync(long currentUserId, DateTime? fromDate, DateTime? toDate);
    Task<List<OrdersByDateResponse>> GetOrdersByDateAsync(long currentUserId);
}

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _context;

    public DashboardService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<OrderStatusStatisticsResponse> GetOrderStatusStatisticsAsync(long currentUserId)
    {
        var response = new OrderStatusStatisticsResponse
        {
            Draft = _context.Orders.Count(o => !o.Deleted && o.CreatedBy == currentUserId && o.Status == OrderStatus.DRAFT),
            New = _context.Orders.Count(o => !o.Deleted && o.CreatedBy == currentUserId && o.Status == OrderStatus.NEW),
            Ordered = _context.Orders.Count(o => !o.Deleted && o.CreatedBy == currentUserId && o.Status == OrderStatus.ORDERED),
            Arrived = _context.Orders.Count(o => !o.Deleted && o.CreatedBy == currentUserId && o.Status == OrderStatus.ARRIVED),
            Cancelled = _context.Orders.Count(o => !o.Deleted && o.CreatedBy == currentUserId && o.Status == OrderStatus.CANCELLED),
            Deleted = _context.Orders.Count(o => o.CreatedBy == currentUserId && o.Deleted)
        };

        return await Task.FromResult(response);
    }

    public async Task<RevenueResponse> GetRevenueAsync(long currentUserId, DateTime? fromDate, DateTime? toDate)
    {
        var query = _context.Orders.Where(o => !o.Deleted && o.CreatedBy == currentUserId);

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

        var totalOrders = query.Count();
        var totalSellingPrice = query.Sum(o => o.AmountSellingPrice);
        var totalImportPrice = query.Sum(o => o.ImportPrice ?? 0);
        var totalWarehousePayment = query.Sum(o => o.WarehousePayment ?? 0);
        var totalShippingFee = query.Sum(o => o.ShippingWeightFee ?? 0);
        var totalRefundAmount = query.Sum(o => o.RefundAmount ?? 0);

        var estimatedProfit = totalSellingPrice - totalImportPrice - totalWarehousePayment - totalShippingFee + totalRefundAmount;

        var response = new RevenueResponse
        {
            TotalOrders = totalOrders,
            TotalSellingPrice = totalSellingPrice,
            TotalImportPrice = totalImportPrice,
            EstimatedProfit = estimatedProfit
        };

        return await Task.FromResult(response);
    }

    public async Task<List<OrdersByDateResponse>> GetOrdersByDateAsync(long currentUserId)
    {
        var grouped = await _context.Orders
            .Where(o => !o.Deleted && o.CreatedBy == currentUserId)
            .GroupBy(o => o.OrderDate.Date)
            .Select(g => new
            {
                Date = g.Key,
                Count = g.Count()
            })
            .OrderBy(x => x.Date)
            .ToListAsync();

        var results = grouped
            .Select(x => new OrdersByDateResponse
            {
                Date = x.Date.ToString("yyyy-MM-dd"),
                Count = x.Count
            })
            .ToList();

        return results;
    }
}

