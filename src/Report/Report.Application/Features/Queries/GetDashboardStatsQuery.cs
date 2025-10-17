using MediatR;
using Microsoft.EntityFrameworkCore;
using Report.Application.Abstractions.Persistence;
using Report.Application.Features.Dtos;


namespace Report.Application.Features.Queries;

public record GetDashboardStatsQuery(DateTime? From, DateTime? To) : IRequest<List<DashboardSummaryDto>>;

public class GetDashboardStatsQueryHandler : IRequestHandler<GetDashboardStatsQuery, List<DashboardSummaryDto>>
{
    private readonly IReportDbContext _db;

    public GetDashboardStatsQueryHandler(IReportDbContext db)
    {
        _db = db;
    }
    public async Task<List<DashboardSummaryDto>> Handle(GetDashboardStatsQuery req, CancellationToken ct)
    {
        var from = req.From ?? DateTime.UtcNow.AddMonths(-1);
        var to = req.To ?? DateTime.UtcNow;

        var summary = new DashboardSummaryDto
        {
            RevenueByDate = await _db.Database
                .SqlQueryRaw<RevenueByDateDto>($"EXEC reporting.sp_TotalRevenueByDate {from}, {to}")
                .ToListAsync(ct),
            RevenueByPaymentProvider = await _db.Database
                .SqlQueryRaw<RevenueByPaymentProviderDto>($"EXEC reporting.sp_RevenueByPaymentProvider {from}, {to}")
                .ToListAsync(ct),
            OrderStatusCounts = await _db.Database
                .SqlQueryRaw<OrderStatusCountDto>("EXEC reporting.sp_CountOrdersByStatus")
                .ToListAsync(ct),
            PaymentEvents = await _db.Database
                .SqlQueryRaw<PaymentEventSummaryDto>("EXEC reporting.sp_PaymentEventSummary")
                .ToListAsync(ct),
        };

        return new List<DashboardSummaryDto> { summary };
    }
}