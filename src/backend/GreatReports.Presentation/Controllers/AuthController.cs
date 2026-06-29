using GreatReports.Application.Common.CQRS;
using GreatReports.Application.UseCases.Auth.Commands;
using GreatReports.Presentation.Requests;
using Microsoft.AspNetCore.Mvc;

namespace GreatReports.Presentation.Controllers;

public class AuthController(ICommandHandler<ConfirmEmailCommand> confirmEmailHandler) : ApiControllerBase
{
    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request, CancellationToken cancellationToken)
    {
        var command = new ConfirmEmailCommand(request.Email, request.Token);
        var result = await confirmEmailHandler.HandleAsync(command, cancellationToken);
        return HandleResult(result);
    }
}
