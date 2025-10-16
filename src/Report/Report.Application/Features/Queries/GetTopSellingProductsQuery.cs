using MediatR;
using ReportService.Application.Abstractions.Persistence;
using ReportService.Application.Features.Dtos;

public record GetTopSellingProductsQuery(int Top) : IRequest<List<TopSellingProductDto>>;

public sealed class GetTopSellingProductsHandler : IRequestHandler<GetTopSellingProductsQuery, List<TopSellingProductDto>>
{
    private readonly IReportRepository _repo;

    public GetTopSellingProductsHandler(IReportRepository repo)
        => _repo = repo;    

    public Task<List<TopSellingProductDto>> Handle(GetTopSellingProductsQuery request, CancellationToken ct)
        => _repo.GetTopSellingProductsAsync(request.Top, ct);
}
