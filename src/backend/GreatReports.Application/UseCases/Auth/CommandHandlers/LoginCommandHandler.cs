using GreatReports.Application.Common.CQRS;
using GreatReports.Application.Common.Interfaces;
using GreatReports.Application.UseCases.Auth.Commands;
using GreatReports.Application.UseCases.Auth.Responses;
using GreatReports.Shared;

namespace GreatReports.Application.UseCases.Auth.CommandHandlers;

public class LoginCommandHandler(IIdentityService identityService) : ICommandHandler<LoginCommand, TokenResponse>
{
    public async Task<Result<TokenResponse>> HandleAsync(LoginCommand command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.Email) || string.IsNullOrWhiteSpace(command.Password))
        {
            return Result.Failure<TokenResponse>(new Error("Auth.InvalidCredentials", "E-mail e senha são obrigatórios."));
        }

        return await identityService.AuthenticateAsync(command.Email, command.Password);
    }
}
