using API.Sale.Data;
using API.Sale.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Sale.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public HealthController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("live")]
    [AllowAnonymous]
    public ActionResult<ApiResponse<object>> GetLiveness()
    {
        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Service is alive",
            Data = new
            {
                status = "healthy",
                timestamp = DateTime.UtcNow
            }
        });
    }

    [HttpGet("ready")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<object>>> GetReadiness()
    {
        var canConnect = await _dbContext.Database.CanConnectAsync();
        if (!canConnect)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new ApiResponse<object>
            {
                Success = false,
                Message = "Service is not ready",
                Data = new
                {
                    status = "unhealthy",
                    database = "disconnected",
                    timestamp = DateTime.UtcNow
                }
            });
        }

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Service is ready",
            Data = new
            {
                status = "healthy",
                database = "connected",
                timestamp = DateTime.UtcNow
            }
        });
    }
}
