using GreatReports.Application.Common.Interfaces;
using GreatReports.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GreatReports.Infrastructure.Persistence.Repositories;

public class GroupRepository : BaseEntityRepository<Group>, IGroupRepository
{
    public GroupRepository(GreatReportsDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IReadOnlyList<Group>> GetGroupsByPartnerIdAsync(Guid partnerId, CancellationToken cancellationToken = default)
    {
        var groupIds = await DbContext.Set<ScheduledEmailReceiver>()
            .Where(r => r.UserId == partnerId && r.Active)
            .Join(DbContext.Set<ScheduledEmail>(),
                r => r.ScheduledEmailId,
                se => se.Id,
                (r, se) => new { se.GroupId, se.Active })
            .Where(x => x.Active)
            .Select(x => x.GroupId)
            .Distinct()
            .ToListAsync(cancellationToken);

        return await DbContext.Set<Group>()
            .Where(g => groupIds.Contains(g.Id) && g.Active)
            .ToListAsync(cancellationToken);
    }
}
