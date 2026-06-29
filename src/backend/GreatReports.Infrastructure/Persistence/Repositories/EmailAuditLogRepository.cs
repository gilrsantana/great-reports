using GreatReports.Application.Common.Interfaces;
using GreatReports.Domain.Entities;

namespace GreatReports.Infrastructure.Persistence.Repositories;

public class EmailAuditLogRepository : BaseEntityRepository<EmailAuditLog>, IEmailAuditLogRepository
{
    public EmailAuditLogRepository(GreatReportsDbContext dbContext) : base(dbContext)
    {
    }
}
