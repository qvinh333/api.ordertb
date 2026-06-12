namespace API.Sale.DTOs.Dashboard;

public class OrderStatusStatisticsResponse
{
    public int Draft { get; set; }
    public int New { get; set; }
    public int Ordered { get; set; }
    public int Arrived { get; set; }
    public int Cancelled { get; set; }
    public int Deleted { get; set; }
}

