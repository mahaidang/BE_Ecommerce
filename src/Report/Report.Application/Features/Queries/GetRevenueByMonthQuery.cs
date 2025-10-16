using MediatR;
using ReportService.Application.Abstractions.Persistence;
using ReportService.Application.Features.Dtos;

public record GetRevenueByMonthQuery(int Year) : IRequest<List<RevenueByMonthDto>>;

public sealed class GetRevenueByMonthHandler : IRequestHandler<GetRevenueByMonthQuery, List<RevenueByMonthDto>>
{

    private readonly IReportRepository _repo;

    public GetRevenueByMonthHandler(IReportRepository repo)
        => _repo = repo;
    public Task<List<RevenueByMonthDto>> Handle(GetRevenueByMonthQuery request, CancellationToken ct)
        => _repo.GetRevenueByMonthAsync(request.Year, ct);
}
