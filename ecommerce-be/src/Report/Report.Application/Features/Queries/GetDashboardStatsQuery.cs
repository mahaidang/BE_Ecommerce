using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Report.Application.Abstractions.Persistence;
using Report.Application.Features.Dtos;


namespace Report.Application.Features.Queries;

public record GetRevenueByDateQuery(DateTime? From, DateTime? To) : IRequest<List<RevenueByDateDto>>;
public record GetRevenueByPaymentProviderQuery(DateTime? From, DateTime? To) : IRequest<List<RevenueByPaymentProviderDto>>;
public record GetOrderStatusCountsQuery : IRequest<List<OrderStatusCountDto>>;
public record GetPaymentEventSummaryQuery : IRequest<List<PaymentEventSummaryDto>>;

public class GetRevenueByDateHandler : IRequestHandler<GetRevenueByDateQuery, List<RevenueByDateDto>>
{
    private readonly IReportDbContext _db;

    public GetRevenueByDateHandler(IReportDbContext db)
    {
        _db = db;
    }
    public async Task<List<RevenueByDateDto>> Handle(GetRevenueByDateQuery req, CancellationToken ct)
    {
        var from = req.From ?? DateTime.UtcNow.AddMonths(-1);
        var to = req.To ?? DateTime.UtcNow;

        var summary = await _db.Database
            .SqlQueryRaw<RevenueByDateDto>(
                "EXEC report.sp_TotalRevenueByDate @from, @to",
                new SqlParameter("@from", from),
                new SqlParameter("@to", to))
            .ToListAsync(ct);

        return summary;
    }
}
public class GetRevenueByPaymentProviderHandler : IRequestHandler<GetRevenueByPaymentProviderQuery, List<RevenueByPaymentProviderDto>>
{
    private readonly IReportDbContext _db;

    public GetRevenueByPaymentProviderHandler(IReportDbContext db)
    {
        _db = db;
    }
    public async Task<List<RevenueByPaymentProviderDto>> Handle(GetRevenueByPaymentProviderQuery req, CancellationToken ct)
    {
        var from = req.From ?? DateTime.UtcNow.AddMonths(-1);
        var to = req.To ?? DateTime.UtcNow;

        var summary = await _db.Database
            .SqlQueryRaw<RevenueByPaymentProviderDto>(
                "EXEC report.sp_RevenueByPaymentProvider @from, @to",
                new SqlParameter("@from", from),
                new SqlParameter("@to", to))
            .ToListAsync(ct);

        return summary;
    }
}
public class GetOrderStatusCountsHandler : IRequestHandler<GetOrderStatusCountsQuery, List<OrderStatusCountDto>>
{
    private readonly IReportDbContext _db;

    public GetOrderStatusCountsHandler(IReportDbContext db)
    {
        _db = db;
    }
    public async Task<List<OrderStatusCountDto>> Handle(GetOrderStatusCountsQuery req, CancellationToken ct)
    {

        var summary = await _db.Database
                   .SqlQueryRaw<OrderStatusCountDto>("EXEC report.sp_CountOrdersByStatus")
                   .ToListAsync(ct);

        return summary;
    }
}
public class GetPaymentEventSummaryHandler : IRequestHandler<GetPaymentEventSummaryQuery, List<PaymentEventSummaryDto>>
{
    private readonly IReportDbContext _db;

    public GetPaymentEventSummaryHandler(IReportDbContext db)
    {
        _db = db;
    }
    public async Task<List<PaymentEventSummaryDto>> Handle(GetPaymentEventSummaryQuery req, CancellationToken ct)
    {
        var summary = await _db.Database
            .SqlQueryRaw<PaymentEventSummaryDto>("EXEC report.sp_PaymentEventSummary")
            .ToListAsync(ct);

        return summary;
    }
}