using GreatReports.Application.Common.Interfaces;
using GreatReports.Domain.Entities;

namespace GreatReports.Infrastructure.Persistence.Repositories;

public class GroupRepository : BaseEntityRepository<Group>, IGroupRepository
{
    public GroupRepository(GreatReportsDbContext dbContext) : base(dbContext)
    {
    }
}
