using GreatReports.Application.Common.CQRS;
using GreatReports.Application.UseCases.Auth.Commands;
using GreatReports.Presentation.Requests;
using Microsoft.AspNetCore.Mvc;

namespace GreatReports.Presentation.Controllers;

public class AuthController : ApiControllerBase
{
    private readonly ICommandHandler<ConfirmEmailCommand> _confirmEmailHandler;

    public AuthController(ICommandHandler<ConfirmEmailCommand> confirmEmailHandler)
    {
        _confirmEmailHandler = confirmEmailHandler;
    }

    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request, CancellationToken cancellationToken)
    {
        var command = new ConfirmEmailCommand(request.Email, request.Token);
        var result = await _confirmEmailHandler.HandleAsync(command, cancellationToken);
        return HandleResult(result);
    }
}
