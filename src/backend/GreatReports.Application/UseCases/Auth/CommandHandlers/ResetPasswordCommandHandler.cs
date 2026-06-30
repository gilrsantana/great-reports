using System.Threading;
using System.Threading.Tasks;
using GreatReports.Application.Common.CQRS;
using GreatReports.Application.Common.Interfaces;
using GreatReports.Application.UseCases.Auth.Commands;
using GreatReports.Shared;

namespace GreatReports.Application.UseCases.Auth.CommandHandlers;
public class ResetPasswordCommandHandler(IIdentityService identityService) : ICommandHandler<ResetPasswordCommand>
{
    public async Task<Result> HandleAsync(ResetPasswordCommand command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.Email) ||
            string.IsNullOrWhiteSpace(command.Token) ||
            string.IsNullOrWhiteSpace(command.NewPassword))
        {
            return Result.Failure(new Error("Auth.InvalidResetRequest", "Todos os campos de redefinição são obrigatórios."));
        }

        return await identityService.ResetPasswordAsync(command.Email, command.Token, command.NewPassword);
    }
}
