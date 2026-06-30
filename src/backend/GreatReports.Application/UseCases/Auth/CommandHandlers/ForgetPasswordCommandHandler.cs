using GreatReports.Application.Common.CQRS;
using GreatReports.Application.Common.Interfaces;
using GreatReports.Application.UseCases.Auth.Commands;
using GreatReports.Shared;

namespace GreatReports.Application.UseCases.Auth.CommandHandlers;

public class ForgetPasswordCommandHandler(IIdentityService identityService) : ICommandHandler<ForgetPasswordCommand>
{
    public async Task<Result> HandleAsync(ForgetPasswordCommand command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.Email))
        {
            return Result.Failure(new Error("Auth.InvalidEmail", "O e-mail é obrigatório."));
        }

        return await identityService.GeneratePasswordResetTokenAsync(command.Email);
    }
}
