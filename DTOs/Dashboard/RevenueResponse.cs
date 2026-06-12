namespace API.Sale.DTOs.Dashboard;

public class RevenueResponse
{
    public int TotalOrders { get; set; }
    public decimal TotalSellingPrice { get; set; }
    public decimal TotalImportPrice { get; set; }
    public decimal EstimatedProfit { get; set; }
}

