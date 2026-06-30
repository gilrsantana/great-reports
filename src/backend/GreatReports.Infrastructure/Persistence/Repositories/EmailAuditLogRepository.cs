using GreatReports.Application.Common.Interfaces;
using GreatReports.Application.Common.Models;
using GreatReports.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GreatReports.Infrastructure.Persistence.Repositories;

public class EmailAuditLogRepository : BaseEntityRepository<EmailAuditLog>, IEmailAuditLogRepository
{
    public EmailAuditLogRepository(GreatReportsDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<PagedResponse<EmailAuditLog>> GetPagedLogsAsync(
        string? recipient,
        bool? success,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = DbContext.Set<EmailAuditLog>().AsQueryable();

        if (!string.IsNullOrWhiteSpace(recipient))
        {
            query = query.Where(l => l.Recipient.Contains(recipient));
        }

        if (success.HasValue)
        {
            query = query.Where(l => l.Success == success.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        
        var items = await query
            .OrderByDescending(l => l.SentAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResponse<EmailAuditLog>(items, page, pageSize, totalCount);
    }
}
