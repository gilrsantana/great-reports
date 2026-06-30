using GreatReports.Application.Common.Interfaces;
using GreatReports.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GreatReports.Infrastructure.Persistence.Repositories;

public class ScheduledEmailRepository : BaseEntityRepository<ScheduledEmail>, IScheduledEmailRepository
{
    public ScheduledEmailRepository(GreatReportsDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IReadOnlyList<ScheduledEmail>> GetByGroupIdAsync(Guid groupId, CancellationToken cancellationToken = default)
    {
        return await DbContext.Set<ScheduledEmail>()
            .Where(e => e.GroupId == groupId && e.Active)
            .ToListAsync(cancellationToken);
    }
}
