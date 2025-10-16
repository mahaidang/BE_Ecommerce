namespace ReportService.Application.Features.Dtos;

public record TopSellingProductDto(Guid ProductId, string ProductName, int QuantitySold, decimal TotalRevenue);

