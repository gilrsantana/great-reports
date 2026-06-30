using GreatReports.Application.Common.CQRS;
using GreatReports.Application.Common.Interfaces;
using GreatReports.Application.UseCases.Auth.Commands;
using GreatReports.Application.UseCases.Auth.Responses;
using GreatReports.Shared;

namespace GreatReports.Application.UseCases.Auth.CommandHandlers;

public class RefreshTokenCommandHandler(IIdentityService identityService) : ICommandHandler<RefreshTokenCommand, TokenResponse>
{
    public async Task<Result<TokenResponse>> HandleAsync(RefreshTokenCommand command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.AccessToken) || string.IsNullOrWhiteSpace(command.RefreshToken))
        {
            return Result.Failure<TokenResponse>(new Error("Auth.InvalidToken", "Tokens de acesso e atualização são obrigatórios."));
        }

        var rotated = await identityService.RotateTokensAsync(command.AccessToken, command.RefreshToken);
        if (rotated == null)
        {
            return Result.Failure<TokenResponse>(new Error("Auth.InvalidRefreshToken", "Token de atualização inválido ou expirado."));
        }

        return new TokenResponse(rotated.Value.AccessToken, rotated.Value.RefreshToken);
    }
}
