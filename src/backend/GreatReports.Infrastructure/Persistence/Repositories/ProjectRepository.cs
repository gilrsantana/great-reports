using GreatReports.Application.Common.Interfaces;
using GreatReports.Domain.Entities;

namespace GreatReports.Infrastructure.Persistence.Repositories;

public class ProjectRepository : BaseEntityRepository<Project>, IProjectRepository
{
    public ProjectRepository(GreatReportsDbContext dbContext) : base(dbContext)
    {
    }
}
