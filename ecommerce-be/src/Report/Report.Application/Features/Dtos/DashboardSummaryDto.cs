namespace Report.Application.Features.Dtos;

public class DashboardSummaryDto
{
    public List<RevenueByDateDto> RevenueByDate { get; set; } = [];
    public List<RevenueByPaymentProviderDto> RevenueByPaymentProvider { get; set; } = [];
    public List<OrderStatusCountDto> OrderStatusCounts { get; set; } = [];
    public List<PaymentEventSummaryDto> PaymentEvents { get; set; } = [];
}

public class RevenueByDateDto
{
    public DateTime Date { get; set; }
    public decimal TotalRevenue { get; set; }
}

public class RevenueByPaymentProviderDto
{
    public string PaymentProvider { get; set; } = string.Empty;
    public int TransactionCount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal Percentage { get; set; }
}

public class OrderStatusCountDto
{
    public string Status { get; set; } = string.Empty;
    public int TotalCount { get; set; }
    public decimal TotalAmount { get; set; }
}

public class PaymentEventSummaryDto
{
    public string EventType { get; set; } = string.Empty;
    public int EventCount { get; set; }
    public DateTime LastOccurred { get; set; }
}