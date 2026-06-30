namespace GreatReports.Application.UseCases.EmailAuditLogs.Queries;

public record EmailAuditLogDto(
    Guid Id,
    string Recipient,
    string Subject,
    string Body,
    DateTimeOffset SentAt,
    bool Success,
    string? ErrorMessage);
