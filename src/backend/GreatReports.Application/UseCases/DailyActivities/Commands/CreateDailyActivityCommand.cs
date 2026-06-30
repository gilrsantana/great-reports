using GreatReports.Application.Common.CQRS;
using GreatReports.Domain.Enums;

namespace GreatReports.Application.UseCases.DailyActivities.Commands;

public record CreateDailyActivityCommand(
    Guid PartnerId,
    string Title,
    string Theme,
    string Content,
    DateTime ReferenceDate,
    ActivityStatus Status,
    bool IsBlocked) : ICommand<Guid>;
