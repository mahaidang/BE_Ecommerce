using Microsoft.EntityFrameworkCore;

namespace Shared.Persistence;

public abstract class AppDbContextBase : DbContext
{
    protected AppDbContextBase(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);
        // chỗ này các DbContext con sẽ gọi mb.HasDefaultSchema("...") theo domain
    }
}
