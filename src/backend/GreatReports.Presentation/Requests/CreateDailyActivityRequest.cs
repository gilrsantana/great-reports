using GreatReports.Domain.Enums;

namespace GreatReports.Presentation.Requests;

public record CreateDailyActivityRequest(
    Guid PartnerId,
    string Title,
    string Theme,
    string Content,
    DateTime ReferenceDate,
    ActivityStatus Status,
    bool IsBlocked);
