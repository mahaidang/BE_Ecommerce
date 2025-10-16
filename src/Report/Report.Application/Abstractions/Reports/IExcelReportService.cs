using ReportService.Application.Features.Dtos;

namespace ReportService.Application.Abstractions.Reports;

public interface IExcelReportService
{
    byte[] GenerateOrderReport(IEnumerable<OrderExportDto> data);
}