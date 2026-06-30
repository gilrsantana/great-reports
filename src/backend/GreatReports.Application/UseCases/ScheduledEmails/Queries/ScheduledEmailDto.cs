using GreatReports.Domain.Enums;

namespace GreatReports.Application.UseCases.ScheduledEmails.Queries;

public record ScheduledEmailDto(
    Guid Id,
    Guid GroupId,
    string Name,
    string CronExpression,
    ReportFrequency Frequency,
    int? SpecificDayOfMonth,
    IReadOnlyList<ScheduledEmailReceiverDto> Receivers);

public record ScheduledEmailReceiverDto(
    Guid Id,
    Guid UserId,
    string UserDisplayName,
    string UserEmail,
    Guid? ClientContactId,
    string? ClientContactName,
    string? ClientContactEmail);
