using MediatR;
using ReportService.Application.Abstractions.Persistence;
using ReportService.Application.Abstractions.Reports;

public record ExportToExcelQuery(DateTime? From, DateTime? To) : IRequest<byte[]>;

public sealed class ExportToExcelHandler : IRequestHandler<ExportToExcelQuery, byte[]>
{
    private readonly IReportRepository _repo;
    private readonly IExcelReportService _excel;

    public ExportToExcelHandler(IReportRepository repo, IExcelReportService excel)
    {
        _repo = repo;
        _excel = excel;
    }
        

    public async Task<byte[]> Handle(ExportToExcelQuery request, CancellationToken ct)
    {
        var data = await _repo.GetOrdersForExportAsync(request.From, request.To, ct);
        return _excel.GenerateOrderReport(data);
    }
}
