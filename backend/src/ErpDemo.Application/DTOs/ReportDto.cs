namespace ErpDemo.Application.DTOs;

public class SummaryReportDto
{
    public int TotalOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public List<OrdersByStatusDto> OrdersByStatus { get; set; } = new();
    public List<TopCustomerDto> TopCustomers { get; set; } = new();
}

public class OrdersByStatusDto
{
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
    public decimal Total { get; set; }
}

public class TopCustomerDto
{
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public int OrderCount { get; set; }
    public decimal TotalSpent { get; set; }
}
