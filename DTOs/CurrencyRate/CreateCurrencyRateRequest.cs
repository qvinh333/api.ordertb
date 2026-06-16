namespace API.Sale.DTOs.CurrencyRate;

public class CreateCurrencyRateRequest
{
    public decimal Rate { get; set; }
    public string? Note { get; set; }
}

