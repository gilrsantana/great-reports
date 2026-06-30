using GreatReports.Application.Common.Interfaces;
using GreatReports.Domain.Entities;

namespace GreatReports.Infrastructure.Persistence.Repositories;

public class ScheduledEmailRepository : BaseEntityRepository<ScheduledEmail>, IScheduledEmailRepository
{
    public ScheduledEmailRepository(GreatReportsDbContext dbContext) : base(dbContext)
    {
    }
}
