using GreatReports.Application.Common.CQRS;
using GreatReports.Application.Common.Interfaces;
using GreatReports.Application.UseCases.Users.Commands;
using GreatReports.Domain.Entities;
using GreatReports.Shared;

namespace GreatReports.Application.UseCases.Users.CommandHandlers;

public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IProviderCompanyRepository _providerCompanyRepository;
    private readonly IIdentityService _identityService;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IProviderCompanyRepository providerCompanyRepository,
        IIdentityService identityService)
    {
        _userRepository = userRepository;
        _providerCompanyRepository = providerCompanyRepository;
        _identityService = identityService;
    }

    public async Task<Result<Guid>> HandleAsync(RegisterUserCommand command, CancellationToken cancellationToken = default)
    {
        var provider = await _providerCompanyRepository.GetByIdAsync(command.ProviderCompanyId, cancellationToken);
        if (provider == null)
        {
            return Result.Failure<Guid>(new Error("ProviderCompany.NotFound", "Provedor não encontrado."));
        }

        var existingUser = await _userRepository.GetByEmailAsync(command.Email, cancellationToken);
        if (existingUser != null)
        {
            return Result.Failure<Guid>(new Error("User.EmailAlreadyExists", "Já existe um usuário cadastrado com este e-mail."));
        }

        var userResult = User.Create(command.ProviderCompanyId, command.DisplayName, command.Email);
        if (userResult.IsFailure)
        {
            return Result.Failure<Guid>(userResult.Error);
        }

        var user = userResult.Value;
        await _userRepository.AddAsync(user, cancellationToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        // 4. Create Identity account
        var tempPassword = Guid.NewGuid().ToString("N") + "aA1!";
        var roles = new[] { command.Role };

        var createAccountSuccess = await _identityService.CreateUserAsync(user.Id, command.Email, tempPassword, roles);

        if (!createAccountSuccess)
        {
            // ROLLBACK: Remove user profile immediately
            _userRepository.Delete(user);
            await _userRepository.SaveChangesAsync(cancellationToken);

            return Result.Failure<Guid>(new Error("User.RegistrationRollback", "Falha ao criar credenciais de autenticação. O cadastro do usuário foi revertido."));
        }

        return user.Id;
    }
}
