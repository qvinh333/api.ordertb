namespace API.Sale.DTOs.Product;

public class CreateProductRequest
{
    public string ProductCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Specification { get; set; }
    public string? Unit { get; set; }
    public decimal? DefaultSellingPrice { get; set; }
    public string? Note { get; set; }
}

