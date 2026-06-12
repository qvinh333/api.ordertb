namespace API.Sale.DTOs.Product;

public class ProductResponse
{
    public long Id { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Specification { get; set; }
    public string? Unit { get; set; }
    public decimal? DefaultSellingPrice { get; set; }
    public string? Note { get; set; }
    public long CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

