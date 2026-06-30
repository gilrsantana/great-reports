using GreatReports.Application.Common.CQRS;
using GreatReports.Domain.Enums;

namespace GreatReports.Application.UseCases.DailyActivities.Queries;

public record DailyActivityDto(
    Guid Id,
    Guid PartnerId,
    string Title,
    string Theme,
    string Content,
    DateTime ReferenceDate,
    ActivityStatus Status,
    bool IsBlocked,
    bool IsPublished,
    DateTime CreatedAt);

public record GetPartnerActivitiesQuery(
    Guid PartnerId,
    string? TitleFilter = null,
    string? ThemeFilter = null,
    ActivityStatus? StatusFilter = null,
    DateTime? DateFilter = null,
    int Page = 1,
    int PageSize = 20) : IQuery<IReadOnlyList<DailyActivityDto>>;
