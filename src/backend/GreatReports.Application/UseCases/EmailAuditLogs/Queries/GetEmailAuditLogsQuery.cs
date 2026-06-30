using GreatReports.Application.Common.CQRS;
using GreatReports.Application.Common.Models;

namespace GreatReports.Application.UseCases.EmailAuditLogs.Queries;

public record GetEmailAuditLogsQuery(
    string? Recipient,
    bool? Success,
    int Page = 1,
    int PageSize = 10) : IQuery<PagedResponse<EmailAuditLogDto>>;
