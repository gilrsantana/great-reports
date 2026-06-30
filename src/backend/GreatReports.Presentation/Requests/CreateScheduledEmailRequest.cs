using GreatReports.Domain.Enums;

namespace GreatReports.Presentation.Requests;

public record CreateScheduledEmailRequest(
    Guid GroupId,
    string Name,
    string CronExpression,
    ReportFrequency Frequency,
    int? SpecificDayOfMonth);
