using GreatReports.Application.Common.CQRS;
using GreatReports.Application.UseCases.Users.Commands;
using GreatReports.Presentation.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreatReports.Presentation.Controllers;

[Authorize(Roles = "Manager")]
public class UsersController(ICommandHandler<RegisterUserCommand, Guid> registerHandler) : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(request.ProviderCompanyId, request.DisplayName, request.Email, request.Role);
        var result = await registerHandler.HandleAsync(command, cancellationToken);
        return HandleResult(result);
    }
}
