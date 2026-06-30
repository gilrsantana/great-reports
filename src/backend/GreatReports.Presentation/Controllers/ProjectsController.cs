using GreatReports.Application.Common.CQRS;
using GreatReports.Application.UseCases.Projects.Commands;
using GreatReports.Presentation.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreatReports.Presentation.Controllers;

[Authorize(Roles = "Manager")]
public class ProjectsController : ApiControllerBase
{
    private readonly ICommandHandler<RegisterProjectCommand, Guid> _registerHandler;

    public ProjectsController(ICommandHandler<RegisterProjectCommand, Guid> registerHandler)
    {
        _registerHandler = registerHandler;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    public async Task<IActionResult> Register([FromBody] RegisterProjectRequest request, CancellationToken cancellationToken)
    {
        var command = new RegisterProjectCommand(request.ClientCompanyId, request.Name, request.Description);
        var result = await _registerHandler.HandleAsync(command, cancellationToken);
        return HandleResult(result);
    }
}
