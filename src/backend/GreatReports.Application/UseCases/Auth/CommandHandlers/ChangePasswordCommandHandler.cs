using GreatReports.Application.Common.CQRS;
using GreatReports.Application.Common.Interfaces;
using GreatReports.Application.UseCases.Auth.Commands;
using GreatReports.Shared;

namespace GreatReports.Application.UseCases.Auth.CommandHandlers;

public class ChangePasswordCommandHandler(IIdentityService identityService) : ICommandHandler<ChangePasswordCommand>
{
    public async Task<Result> HandleAsync(ChangePasswordCommand command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.CurrentPassword) || string.IsNullOrWhiteSpace(command.NewPassword))
        {
            return Result.Failure(new Error("Auth.InvalidPassword", "As senhas atual e nova são obrigatórias."));
        }

        return await identityService.ChangePasswordAsync(command.AccountId, command.CurrentPassword, command.NewPassword);
    }
}
