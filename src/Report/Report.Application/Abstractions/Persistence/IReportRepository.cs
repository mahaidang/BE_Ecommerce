using ReportService.Application.Features.Dtos;

namespace ReportService.Application.Abstractions.Persistence;
public interface IReportRepository
{
    Task<List<RevenueByMonthDto>> GetRevenueByMonthAsync(int year, CancellationToken ct);
    Task<List<TopSellingProductDto>> GetTopSellingProductsAsync(int top, CancellationToken ct);
    Task<List<OrderExportDto>> GetOrdersForExportAsync(DateTime? from, DateTime? to, CancellationToken ct);
}

