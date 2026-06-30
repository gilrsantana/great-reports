using GreatReports.Application.Common.CQRS;
using GreatReports.Application.UseCases.Users.Commands;
using GreatReports.Application.UseCases.Users.Queries;
using GreatReports.Presentation.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreatReports.Presentation.Controllers;

[Authorize(Roles = "Manager")]
public class UsersController(
    ICommandHandler<RegisterUserCommand, Guid> registerHandler,
    IQueryHandler<GetUsersQuery, IReadOnlyList<UserDto>> listHandler) : ApiControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(request.ProviderCompanyId, request.DisplayName, request.Email, request.Role);
        var result = await registerHandler.HandleAsync(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<UserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromQuery] Guid providerId, CancellationToken cancellationToken)
    {
        var query = new GetUsersQuery(providerId);
        var result = await listHandler.HandleAsync(query, cancellationToken);
        return HandleResult(result);
    }
}
