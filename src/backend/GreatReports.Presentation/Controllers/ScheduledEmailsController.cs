using GreatReports.Application.Common.CQRS;
using GreatReports.Application.UseCases.ScheduledEmails.Commands;
using GreatReports.Application.UseCases.ScheduledEmails.Queries;
using GreatReports.Presentation.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GreatReports.Presentation.Controllers;

[Authorize(Roles = "GroupLeader,Manager")]
public class ScheduledEmailsController(
    ICommandHandler<CreateScheduledEmailCommand, Guid> createHandler,
    ICommandHandler<AddScheduledEmailReceiverCommand> addReceiverHandler,
    IQueryHandler<GetGroupScheduledEmailsQuery, IReadOnlyList<ScheduledEmailDto>> getSchedulesHandler) : ApiControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    public async Task<IActionResult> Create([FromBody] CreateScheduledEmailRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateScheduledEmailCommand(
            request.GroupId,
            request.Name,
            request.CronExpression,
            request.Frequency,
            request.SpecificDayOfMonth);
        var result = await createHandler.HandleAsync(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{id:guid}/receivers")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AddReceiver(Guid id, [FromBody] AddScheduledEmailReceiverRequest request, CancellationToken cancellationToken)
    {
        var command = new AddScheduledEmailReceiverCommand(id, request.UserId, request.ClientContactId);
        var result = await addReceiverHandler.HandleAsync(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("group/{groupId:guid}")]
    [ProducesResponseType(typeof(IReadOnlyList<ScheduledEmailDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetGroupSchedules(Guid groupId, CancellationToken cancellationToken)
    {
        var query = new GetGroupScheduledEmailsQuery(groupId);
        var result = await getSchedulesHandler.HandleAsync(query, cancellationToken);
        return HandleResult(result);
    }
}
