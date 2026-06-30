using System.Security.Claims;
using GreatReports.Application.Common.CQRS;
using GreatReports.Application.UseCases.Auth.Commands;
using GreatReports.Application.UseCases.Auth.Responses;
using GreatReports.Presentation.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreatReports.Presentation.Controllers;

public class AuthController(
    ICommandHandler<ConfirmEmailCommand> confirmEmailHandler,
    ICommandHandler<LoginCommand, TokenResponse> loginHandler,
    ICommandHandler<RefreshTokenCommand, TokenResponse> refreshTokenHandler,
    ICommandHandler<ChangePasswordCommand> changePasswordHandler,
    ICommandHandler<ForgetPasswordCommand> forgetPasswordHandler,
    ICommandHandler<ResetPasswordCommand> resetPasswordHandler) : ApiControllerBase
{
    [HttpPost("confirm-email")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest request, CancellationToken cancellationToken)
    {
        var command = new ConfirmEmailCommand(request.Email, request.Token);
        var result = await confirmEmailHandler.HandleAsync(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var command = new LoginCommand(request.Email, request.Password);
        var result = await loginHandler.HandleAsync(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("refresh-token")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var command = new RefreshTokenCommand(request.AccessToken, request.RefreshToken);
        var result = await refreshTokenHandler.HandleAsync(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var accountId))
        {
            return Unauthorized();
        }

        var command = new ChangePasswordCommand(accountId, request.CurrentPassword, request.NewPassword);
        var result = await changePasswordHandler.HandleAsync(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("forget-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordRequest request, CancellationToken cancellationToken)
    {
        var command = new ForgetPasswordCommand(request.Email);
        var result = await forgetPasswordHandler.HandleAsync(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken cancellationToken)
    {
        var command = new ResetPasswordCommand(request.Email, request.Token, request.NewPassword);
        var result = await resetPasswordHandler.HandleAsync(command, cancellationToken);
        return HandleResult(result);
    }
}
