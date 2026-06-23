namespace API.Sale.DTOs.Order;

public class OrderMoneySummaryResponse
{
    public int TotalOrders { get; set; }
    public int TotalQuantity { get; set; }
    public decimal TotalSellingPrice { get; set; }
    public decimal TotalAmountSellingPrice { get; set; }
    public decimal TotalYuanPrice { get; set; }
    public decimal TotalImportPrice { get; set; }
    public decimal TotalWarehousePayment { get; set; }
    public decimal TotalShippingWeightFee { get; set; }
    public decimal TotalRefundAmount { get; set; }
    public decimal EstimatedProfit { get; set; }
}
