using GreatReports.Application.Common.Interfaces;
using GreatReports.Domain.Entities;

namespace GreatReports.Infrastructure.Persistence.Repositories;

public class DailyActivityRepository : BaseEntityRepository<DailyActivity>, IDailyActivityRepository
{
    public DailyActivityRepository(GreatReportsDbContext dbContext) : base(dbContext)
    {
    }
}
