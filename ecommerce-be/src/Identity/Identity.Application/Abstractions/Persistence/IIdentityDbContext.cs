using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Application.Abstractions.Persistence;

public interface IIdentityDbContext
{
    DbSet<User> Users { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
