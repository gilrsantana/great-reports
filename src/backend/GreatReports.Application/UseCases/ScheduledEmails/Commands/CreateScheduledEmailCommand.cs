using GreatReports.Application.Common.CQRS;
using GreatReports.Domain.Enums;

namespace GreatReports.Application.UseCases.ScheduledEmails.Commands;

public record CreateScheduledEmailCommand(
    Guid GroupId,
    string Name,
    string CronExpression,
    ReportFrequency Frequency,
    int? SpecificDayOfMonth) : ICommand<Guid>;
