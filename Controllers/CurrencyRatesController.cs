using API.Sale.DTOs;
using API.Sale.DTOs.CurrencyRate;
using API.Sale.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Sale.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN,SELLER")]
public class CurrencyRatesController : ControllerBase
{
    private readonly ICurrencyRateService _currencyRateService;
    private readonly CurrentUserService _currentUserService;

    public CurrencyRatesController(ICurrencyRateService currencyRateService, CurrentUserService currentUserService)
    {
        _currencyRateService = currencyRateService;
        _currentUserService = currentUserService;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<CurrencyRateResponse>>> ConfigureRate([FromBody] CreateCurrencyRateRequest request)
    {
        if (request.Rate <= 0)
        {
            return BadRequest(new ApiResponse<CurrencyRateResponse>
            {
                Success = false,
                Message = "Rate must be greater than 0"
            });
        }

        var userId = _currentUserService.GetUserId();
        var rate = await _currencyRateService.CreateRateAsync(request, userId);

        return Ok(new ApiResponse<CurrencyRateResponse>
        {
            Success = true,
            Message = "Currency rate configured successfully",
            Data = MapToResponse(rate)
        });
    }

    [HttpGet("latest")]
    public async Task<ActionResult<ApiResponse<CurrencyRateResponse>>> GetLatestRate()
    {
        var userId = _currentUserService.GetUserId();
        var rate = await _currencyRateService.GetLatestRateAsync(userId);

        if (rate == null)
        {
            return NotFound(new ApiResponse<CurrencyRateResponse>
            {
                Success = false,
                Message = "No currency rate configured"
            });
        }

        return Ok(new ApiResponse<CurrencyRateResponse>
        {
            Success = true,
            Message = "Success",
            Data = MapToResponse(rate)
        });
    }

    private static CurrencyRateResponse MapToResponse(Models.CurrencyRateHistory rate)
    {
        return new CurrencyRateResponse
        {
            Id = rate.Id,
            Rate = rate.Rate,
            Note = rate.Note,
            CreatedBy = rate.CreatedBy,
            CreatedAt = rate.CreatedAt
        };
    }
}

