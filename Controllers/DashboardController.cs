using API.Sale.DTOs;
using API.Sale.DTOs.Dashboard;
using API.Sale.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Sale.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN,SELLER")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;
    private readonly CurrentUserService _currentUserService;

    public DashboardController(IDashboardService dashboardService, CurrentUserService currentUserService)
    {
        _dashboardService = dashboardService;
        _currentUserService = currentUserService;
    }

    [HttpGet("order-status")]
    public async Task<ActionResult<ApiResponse<OrderStatusStatisticsResponse>>> GetOrderStatusStatistics()
    {
        var userId = _currentUserService.GetUserId();
        var result = await _dashboardService.GetOrderStatusStatisticsAsync(userId);

        return Ok(new ApiResponse<OrderStatusStatisticsResponse>
        {
            Success = true,
            Message = "Success",
            Data = result
        });
    }

    [HttpGet("revenue")]
    public async Task<ActionResult<ApiResponse<RevenueResponse>>> GetRevenue(
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null)
    {
        var userId = _currentUserService.GetUserId();
        var result = await _dashboardService.GetRevenueAsync(userId, fromDate, toDate);

        return Ok(new ApiResponse<RevenueResponse>
        {
            Success = true,
            Message = "Success",
            Data = result
        });
    }

    [HttpGet("orders-by-date")]
    public async Task<ActionResult<ApiResponse<List<OrdersByDateResponse>>>> GetOrdersByDate()
    {
        var userId = _currentUserService.GetUserId();
        var result = await _dashboardService.GetOrdersByDateAsync(userId);

        return Ok(new ApiResponse<List<OrdersByDateResponse>>
        {
            Success = true,
            Message = "Success",
            Data = result
        });
    }
}

