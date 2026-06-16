namespace API.Sale.Models;

public class CurrencyRateHistory
{
    public long Id { get; set; }
    public decimal Rate { get; set; }
    public string? Note { get; set; }
    public long CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

