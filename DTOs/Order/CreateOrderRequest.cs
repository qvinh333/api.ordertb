namespace API.Sale.DTOs.Order;

public class CreateOrderRequest
{
    public string OrderCode { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string? Specification { get; set; }
    public int Quantity { get; set; }
    public decimal SellingPrice { get; set; }
    public decimal AmountSellingPrice { get; set; }
    public string Status { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public decimal? YuanPrice { get; set; }
    public decimal? ImportPrice { get; set; }
    public string? Supplier { get; set; }
    public decimal? WarehousePayment { get; set; }
    public decimal? ShippingWeightFee { get; set; }
    public DateTime? ShippingPaymentDate { get; set; }
    public decimal? RefundAmount { get; set; }
    public string? RefundStatus { get; set; }
    public string? Note { get; set; }
}

