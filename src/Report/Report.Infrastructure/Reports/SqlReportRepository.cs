using Microsoft.EntityFrameworkCore;
using ReportService.Application.Abstractions.Persistence;
using ReportService.Application.Features.Dtos;
using ReportService.Infrastructure.Persistence;
using System.Linq;

namespace ReportService.Infrastructure.Reports;

public sealed class SqlReportRepository : IReportRepository
{
    private readonly ReportDbContext _db;

    public SqlReportRepository(ReportDbContext db)
    {
        _db = db;
    }

    public async Task<List<RevenueByMonthDto>> GetRevenueByMonthAsync(int year, CancellationToken ct)
    {
        // EF Core không thể dịch trực tiếp thuộc tính Year/Month của DateTime trong truy vấn SQL.
        // Sử dụng truy vấn tách riêng năm/tháng bằng cách dùng thuộc tính .Year/.Month ngoài truy vấn DB.
        return await Task.Run(() =>
            _db.ReportOrders
                .Where(o => o.Status == "Paid" && o.CreatedAt.Year == year)
                .AsEnumerable() // Chuyển sang xử lý phía client
                .GroupBy(o => o.CreatedAt.Month)
                .Select(g => new RevenueByMonthDto(g.Key, g.Sum(x => x.Total)))
                .OrderBy(x => x.Month)
                .ToList(),
            ct
        );
    }

    public async Task<List<TopSellingProductDto>> GetTopSellingProductsAsync(int top, CancellationToken ct)
    {
        return await _db.ReportProductStats
            .OrderByDescending(x => x.TotalRevenue)
            .Take(top)
            .Select(x => new TopSellingProductDto(x.ProductId, x.ProductName, x.QuantitySold, x.TotalRevenue))
            .ToListAsync(ct);
    }

    public async Task<List<OrderExportDto>> GetOrdersForExportAsync(DateTime? from, DateTime? to, CancellationToken ct)
    {
        var q = _db.ReportOrders.AsQueryable();
        if (from.HasValue) q = q.Where(x => x.CreatedAt >= from);
        if (to.HasValue) q = q.Where(x => x.CreatedAt <= to);

        return await q.OrderByDescending(x => x.CreatedAt)
            .Select(x => new OrderExportDto(x.Id, x.UserId.ToString(), x.CreatedAt, x.Total, x.Status))
            .ToListAsync(ct);
    }
}
