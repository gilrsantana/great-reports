using GreatReports.Application.Common.CQRS;
using GreatReports.Application.Common.Interfaces;
using GreatReports.Application.Common.Models;
using GreatReports.Application.UseCases.EmailAuditLogs.Queries;
using GreatReports.Shared;

namespace GreatReports.Application.UseCases.EmailAuditLogs.QueryHandlers;

public class GetEmailAuditLogsQueryHandler(IEmailAuditLogRepository repository)
    : IQueryHandler<GetEmailAuditLogsQuery, PagedResponse<EmailAuditLogDto>>
{
    public async Task<Result<PagedResponse<EmailAuditLogDto>>> HandleAsync(
        GetEmailAuditLogsQuery query, CancellationToken cancellationToken = default)
    {
        var pagedLogs = await repository.GetPagedLogsAsync(
            query.Recipient,
            query.Success,
            query.Page,
            query.PageSize,
            cancellationToken);

        var dtos = pagedLogs.Items.Select(l => new EmailAuditLogDto(
            l.Id,
            l.Recipient,
            l.Subject,
            l.Body,
            l.SentAt,
            l.Success,
            l.ErrorMessage
        )).ToList();

        var response = new PagedResponse<EmailAuditLogDto>(dtos, pagedLogs.Page, pagedLogs.PageSize, pagedLogs.TotalCount);
        return Result.Success(response);
    }
}
