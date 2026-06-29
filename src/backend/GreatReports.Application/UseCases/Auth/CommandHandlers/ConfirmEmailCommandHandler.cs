using GreatReports.Application.Common.CQRS;
using GreatReports.Application.Common.Interfaces;
using GreatReports.Application.UseCases.Auth.Commands;
using GreatReports.Shared;

namespace GreatReports.Application.UseCases.Auth.CommandHandlers;

public class ConfirmEmailCommandHandler : ICommandHandler<ConfirmEmailCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IClientContactRepository _clientContactRepository;
    private readonly IIdentityService _identityService;

    public ConfirmEmailCommandHandler(
        IUserRepository userRepository,
        IClientContactRepository clientContactRepository,
        IIdentityService identityService)
    {
        _userRepository = userRepository;
        _clientContactRepository = clientContactRepository;
        _identityService = identityService;
    }

    public async Task<Result> HandleAsync(ConfirmEmailCommand command, CancellationToken cancellationToken = default)
    {
        // 1. Search in target user profile
        var user = await _userRepository.GetByEmailAsync(command.Email, cancellationToken);
        if (user != null)
        {
            if (user.EmailConfirmed)
            {
                return Result.Success();
            }

            var confirmed = await _identityService.ConfirmEmailAsync(user.Id, command.Token);
            if (!confirmed)
            {
                return Result.Failure(new Error("Auth.InvalidToken", "O token de confirmação de e-mail é inválido."));
            }

            user.ConfirmEmail();
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }

        // 2. Search in client contact profile
        var contact = await _clientContactRepository.GetByEmailAsync(command.Email, cancellationToken);
        if (contact != null)
        {
            var confirmed = await _identityService.ConfirmEmailAsync(contact.Id, command.Token);
            if (!confirmed)
            {
                return Result.Failure(new Error("Auth.InvalidToken", "O token de confirmação de e-mail é inválido."));
            }

            return Result.Success();
        }

        return Result.Failure(new Error("Auth.UserNotFound", "Nenhum cadastro foi encontrado com o e-mail fornecido."));
    }
}
