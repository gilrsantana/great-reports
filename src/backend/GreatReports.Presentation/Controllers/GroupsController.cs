using GreatReports.Application.Common.CQRS;
using GreatReports.Application.UseCases.Groups.Commands;
using GreatReports.Application.UseCases.Groups.Queries;
using GreatReports.Presentation.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GreatReports.Presentation.Controllers;

[Authorize(Roles = "GroupLeader,Manager")]
public class GroupsController(
    ICommandHandler<CreateGroupCommand, Guid> createHandler,
    IQueryHandler<GetGroupsQuery, IReadOnlyList<GroupDto>> getGroupsHandler,
    IQueryHandler<GetGroupByIdQuery, GroupDto> getGroupByIdHandler) : ApiControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    public async Task<IActionResult> Create([FromBody] CreateGroupRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateGroupCommand(
            request.GroupLeaderId,
            request.ClientCompanyId,
            request.ProjectId,
            request.Name,
            request.Timezone);
        var result = await createHandler.HandleAsync(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<GroupDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] Guid? groupLeaderId, CancellationToken cancellationToken)
    {
        var query = new GetGroupsQuery(groupLeaderId);
        var result = await getGroupsHandler.HandleAsync(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GroupDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetGroupByIdQuery(id);
        var result = await getGroupByIdHandler.HandleAsync(query, cancellationToken);
        return HandleResult(result);
    }
}
