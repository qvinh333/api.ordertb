namespace API.Sale.DTOs.Product;

public class UpdateProductRequest
{
    public string? ProductCode { get; set; }
    public string? Name { get; set; }
    public string? Specification { get; set; }
    public string? Unit { get; set; }
    public decimal? DefaultSellingPrice { get; set; }
    public string? Note { get; set; }
}

