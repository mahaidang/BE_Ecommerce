namespace ReportService.Application.Features.Dtos;

public record OrderExportDto(Guid OrderId, string Customer, DateTime CreatedAt, decimal Total, string Status);
