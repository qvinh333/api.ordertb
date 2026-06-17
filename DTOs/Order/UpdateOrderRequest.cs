namespace API.Sale.DTOs.Order;

public class UpdateOrderRequest
{
    public string? OrderCode { get; set; }
    public DateTime? OrderDate { get; set; }
    public string? CustomerName { get; set; }
    public string? ProductName { get; set; }
    public string? Specification { get; set; }
    public int? Quantity { get; set; }
    public decimal? SellingPrice { get; set; }
    public decimal? AmountSellingPrice { get; set; }
    public string? PaymentStatus { get; set; }
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

