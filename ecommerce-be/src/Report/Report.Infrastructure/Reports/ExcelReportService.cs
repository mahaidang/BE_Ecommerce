using OfficeOpenXml;
using OfficeOpenXml.Style;
using ReportService.Application.Abstractions.Reports;
using ReportService.Application.Features.Dtos;

namespace ReportService.Infrastructure.Reports;

public sealed class ExcelReportService : IExcelReportService
{
    public byte[] GenerateOrderReport(IEnumerable<OrderExportDto> data)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        using var pkg = new ExcelPackage();
        var ws = pkg.Workbook.Worksheets.Add("Orders");

        // Header
        ws.Cells[1, 1].Value = "Order ID";
        ws.Cells[1, 2].Value = "Customer";
        ws.Cells[1, 3].Value = "Created At";
        ws.Cells[1, 4].Value = "Total (VND)";
        ws.Cells[1, 5].Value = "Status";

        using (var r = ws.Cells[1, 1, 1, 5])
        {
            r.Style.Font.Bold = true;
            r.Style.Fill.PatternType = ExcelFillStyle.Solid;
            r.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        }

        var row = 2;
        foreach (var o in data)
        {
            ws.Cells[row, 1].Value = o.OrderId.ToString();
            ws.Cells[row, 2].Value = o.Customer;
            ws.Cells[row, 3].Value = o.CreatedAt.ToString("yyyy-MM-dd HH:mm");
            ws.Cells[row, 4].Value = o.Total;
            ws.Cells[row, 5].Value = o.Status;
            row++;
        }

        ws.Cells.AutoFitColumns();
        return pkg.GetAsByteArray();
    }
}
