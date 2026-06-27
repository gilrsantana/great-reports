using GreatReports.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GreatReports.Infrastructure.Persistence;

public class GreatReportsDbContext : IdentityDbContext<Account, Role, Guid>
{
    public GreatReportsDbContext(DbContextOptions<GreatReportsDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(typeof(GreatReportsDbContext).Assembly);
    }
}
