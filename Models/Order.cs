using API.Sale.Models.Enums;

namespace API.Sale.Models;

public class Order
{
    public long Id { get; set; }
    public string OrderCode { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string? Specification { get; set; }
    public int Quantity { get; set; }
    public decimal SellingPrice { get; set; }
    public decimal AmountSellingPrice { get; set; }
    public OrderStatus Status { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public decimal? YuanPrice { get; set; }
    public decimal? ImportPrice { get; set; }
    public string? Supplier { get; set; }
    public decimal? WarehousePayment { get; set; }
    public decimal? ShippingWeightFee { get; set; }
    public DateTime? ShippingPaymentDate { get; set; }
    public decimal? RefundAmount { get; set; }
    public string? RefundStatus { get; set; }
    public string? Note { get; set; }
    public long CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool Deleted { get; set; } = false;
}

