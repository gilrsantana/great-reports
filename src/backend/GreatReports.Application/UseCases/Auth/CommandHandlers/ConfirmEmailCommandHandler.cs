using GreatReports.Application.Common.CQRS;
using GreatReports.Application.Common.Interfaces;
using GreatReports.Application.UseCases.Auth.Commands;
using GreatReports.Shared;

namespace GreatReports.Application.UseCases.Auth.CommandHandlers;

public class ConfirmEmailCommandHandler(
    IUserRepository userRepository,
    IClientContactRepository clientContactRepository,
    IIdentityService identityService) : ICommandHandler<ConfirmEmailCommand>
{
    public async Task<Result> HandleAsync(ConfirmEmailCommand command, CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetByEmailAsync(command.Email, cancellationToken);
        if (user != null)
        {
            if (user.EmailConfirmed)
            {
                return Result.Success();
            }

            var confirmed = await identityService.ConfirmEmailAsync(user.Id, command.Token);
            if (!confirmed)
            {
                return Result.Failure(new Error("Auth.InvalidToken", "O token de confirmação de e-mail é inválido."));
            }

            user.ConfirmEmail();
            userRepository.Update(user);
            await userRepository.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        // 2. Search in client contact profile
        var contact = await clientContactRepository.GetByEmailAsync(command.Email, cancellationToken);
        if (contact != null)
        {
            var confirmed = await identityService.ConfirmEmailAsync(contact.Id, command.Token);
            if (!confirmed)
            {
                return Result.Failure(new Error("Auth.InvalidToken", "O token de confirmação de e-mail é inválido."));
            }

            return Result.Success();
        }

        return Result.Failure(new Error("Auth.UserNotFound", "Nenhum cadastro foi encontrado com o e-mail fornecido."));
    }
}
