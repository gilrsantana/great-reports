using GreatReports.Application.Common.Interfaces;
using GreatReports.Domain.Entities;

namespace GreatReports.Infrastructure.Persistence.Repositories;

public class ScheduledEmailReceiverRepository : BaseEntityRepository<ScheduledEmailReceiver>, IScheduledEmailReceiverRepository
{
    public ScheduledEmailReceiverRepository(GreatReportsDbContext dbContext) : base(dbContext)
    {
    }
}
