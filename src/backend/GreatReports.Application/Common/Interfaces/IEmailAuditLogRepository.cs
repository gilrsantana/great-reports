using GreatReports.Application.Common.Models;
using GreatReports.Domain.Entities;

namespace GreatReports.Application.Common.Interfaces;

public interface IEmailAuditLogRepository : IRepository<EmailAuditLog>
{
    Task<PagedResponse<EmailAuditLog>> GetPagedLogsAsync(
        string? recipient,
        bool? success,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}
