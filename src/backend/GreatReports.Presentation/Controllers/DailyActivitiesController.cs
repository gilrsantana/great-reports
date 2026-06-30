using GreatReports.Application.Common.CQRS;
using GreatReports.Application.UseCases.DailyActivities.Commands;
using GreatReports.Application.UseCases.DailyActivities.Queries;
using GreatReports.Domain.Enums;
using GreatReports.Presentation.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using GreatReports.Application.Common.Interfaces;

namespace GreatReports.Presentation.Controllers;

[Authorize(Roles = "Partner")]
public class DailyActivitiesController(
    ICommandHandler<CreateDailyActivityCommand, Guid> createHandler,
    ICommandHandler<UpdateDailyActivityCommand> updateHandler,
    IQueryHandler<GetPartnerActivitiesQuery, IReadOnlyList<DailyActivityDto>> getActivitiesHandler,
    IGroupRepository groupRepository,
    IDailyActivityRepository dailyActivityRepository) : ApiControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    public async Task<IActionResult> Create([FromBody] CreateDailyActivityRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateDailyActivityCommand(
            request.PartnerId,
            request.Title,
            request.Theme,
            request.Content,
            request.ReferenceDate,
            request.Status,
            request.IsBlocked);
        var result = await createHandler.HandleAsync(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDailyActivityRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateDailyActivityCommand(
            id,
            request.PartnerId,
            request.Title,
            request.Theme,
            request.Content,
            request.ReferenceDate,
            request.Status,
            request.IsBlocked);
        var result = await updateHandler.HandleAsync(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("lockout-status")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLockoutStatus([FromQuery] Guid partnerId, CancellationToken cancellationToken)
    {
        var groups = await groupRepository.GetGroupsByPartnerIdAsync(partnerId, cancellationToken);
        foreach (var group in groups)
        {
            var timezoneInfo = TimeZoneInfo.FindSystemTimeZoneById(group.Timezone);
            var localTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timezoneInfo);
            if (localTimeNow.Hour > 23 || (localTimeNow.Hour == 23 && localTimeNow.Minute >= 50))
            {
                return Ok(true);
            }
        }
        return Ok(false);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(DailyActivityDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var activity = await dailyActivityRepository.GetByIdAsync(id, cancellationToken);
        if (activity == null)
        {
            return NotFound();
        }
        var dto = new DailyActivityDto(
            activity.Id,
            activity.PartnerId,
            activity.Title,
            activity.Theme,
            activity.Content,
            activity.ReferenceDate,
            activity.Status,
            activity.IsBlocked,
            activity.IsPublished,
            activity.CreatedAt);
        return Ok(dto);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<DailyActivityDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActivities(
        [FromQuery] Guid partnerId,
        [FromQuery] string? title,
        [FromQuery] string? theme,
        [FromQuery] ActivityStatus? status,
        [FromQuery] DateTime? date,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new GetPartnerActivitiesQuery(partnerId, title, theme, status, date, page, pageSize);
        var result = await getActivitiesHandler.HandleAsync(query, cancellationToken);
        return HandleResult(result);
    }
}
