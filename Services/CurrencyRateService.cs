using API.Sale.Data;
using API.Sale.DTOs.CurrencyRate;
using API.Sale.Models;

namespace API.Sale.Services;

public interface ICurrencyRateService
{
    Task<CurrencyRateHistory> CreateRateAsync(CreateCurrencyRateRequest request, long currentUserId);
    Task<CurrencyRateHistory?> GetLatestRateAsync(long currentUserId);
    Task<List<CurrencyRateHistory>> GetRatesAsync(long currentUserId);
}

public class CurrencyRateService : ICurrencyRateService
{
    private readonly AppDbContext _context;

    public CurrencyRateService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CurrencyRateHistory> CreateRateAsync(CreateCurrencyRateRequest request, long currentUserId)
    {
        if (request.Rate <= 0)
        {
            throw new InvalidOperationException("Rate must be greater than 0");
        }

        var rate = new CurrencyRateHistory
        {
            Rate = request.Rate,
            Note = request.Note,
            CreatedBy = currentUserId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Set<CurrencyRateHistory>().Add(rate);
        await _context.SaveChangesAsync();

        return rate;
    }

    public Task<CurrencyRateHistory?> GetLatestRateAsync(long currentUserId)
    {
        var latestRate = _context.Set<CurrencyRateHistory>()
            .Where(x => x.CreatedBy == currentUserId)
            .OrderByDescending(x => x.CreatedAt)
            .ThenByDescending(x => x.Id)
            .FirstOrDefault();

        return Task.FromResult(latestRate);
    }

    public Task<List<CurrencyRateHistory>> GetRatesAsync(long currentUserId)
    {
        var rates = _context.Set<CurrencyRateHistory>()
            .Where(x => x.CreatedBy == currentUserId)
            .OrderByDescending(x => x.CreatedAt)
            .ThenByDescending(x => x.Id)
            .ToList();

        return Task.FromResult(rates);
    }
}
