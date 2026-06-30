using GreatReports.Application.Common.CQRS;
using GreatReports.Application.Common.Interfaces;
using GreatReports.Application.UseCases.DailyActivities.Queries;
using GreatReports.Shared;

namespace GreatReports.Application.UseCases.DailyActivities.QueryHandlers;

public class GetPartnerActivitiesQueryHandler(IDailyActivityRepository dailyActivityRepository)
    : IQueryHandler<GetPartnerActivitiesQuery, IReadOnlyList<DailyActivityDto>>
{
    public async Task<Result<IReadOnlyList<DailyActivityDto>>> HandleAsync(
        GetPartnerActivitiesQuery query, CancellationToken cancellationToken = default)
    {
        var all = await dailyActivityRepository.GetAllAsync(cancellationToken);

        var filtered = all
            .Where(a => a.PartnerId == query.PartnerId)
            .AsEnumerable();

        if (!string.IsNullOrWhiteSpace(query.TitleFilter))
            filtered = filtered.Where(a => a.Title.Contains(query.TitleFilter, StringComparison.OrdinalIgnoreCase));

        if (!string.IsNullOrWhiteSpace(query.ThemeFilter))
            filtered = filtered.Where(a => a.Theme.Contains(query.ThemeFilter, StringComparison.OrdinalIgnoreCase));

        if (query.StatusFilter.HasValue)
            filtered = filtered.Where(a => a.Status == query.StatusFilter.Value);

        if (query.DateFilter.HasValue)
            filtered = filtered.Where(a => a.ReferenceDate.Date == query.DateFilter.Value.Date);

        var dtos = filtered
            .OrderByDescending(a => a.ReferenceDate)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(a => new DailyActivityDto(
                a.Id,
                a.PartnerId,
                a.Title,
                a.Theme,
                a.Content,
                a.ReferenceDate,
                a.Status,
                a.IsBlocked,
                a.IsPublished,
                a.CreatedAt))
            .ToList();

        return Result.Success<IReadOnlyList<DailyActivityDto>>(dtos);
    }
}
