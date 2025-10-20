using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Report.Application.Features.Dtos;
using ReportService.Domain.Entities;

namespace Report.Application.Abstractions.Persistence;

public interface IReportDbContext
{
    DbSet<ReportOrder> ReportOrders { get; }
    DbSet<ReportProductStat> ReportProductStats { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);

    DatabaseFacade Database { get; }
}
