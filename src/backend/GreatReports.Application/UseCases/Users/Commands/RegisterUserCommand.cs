using GreatReports.Application.Common.CQRS;
using GreatReports.Application.Common.Interfaces;
using GreatReports.Domain.Entities;
using GreatReports.Shared;

namespace GreatReports.Application.UseCases.Users.Commands;

public record RegisterUserCommand(Guid ProviderCompanyId, string DisplayName, string Email, string Role) : ICommand<Guid>;

public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IProviderCompanyRepository _providerCompanyRepository;
    private readonly IIdentityService _identityService;
    private readonly IEmailVerificationService _emailVerificationService;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IProviderCompanyRepository providerCompanyRepository,
        IIdentityService identityService,
        IEmailVerificationService emailVerificationService)
    {
        _userRepository = userRepository;
        _providerCompanyRepository = providerCompanyRepository;
        _identityService = identityService;
        _emailVerificationService = emailVerificationService;
    }

    public async Task<Result<Guid>> HandleAsync(RegisterUserCommand command, CancellationToken cancellationToken = default)
    {
        // 1. Verify provider company exists
        var provider = await _providerCompanyRepository.GetByIdAsync(command.ProviderCompanyId, cancellationToken);
        if (provider == null)
        {
            return Result.Failure<Guid>(new Error("ProviderCompany.NotFound", "Provedor não encontrado."));
        }

        // 2. Check if user already exists
        var existingUser = await _userRepository.GetByEmailAsync(command.Email, cancellationToken);
        if (existingUser != null)
        {
            return Result.Failure<Guid>(new Error("User.EmailAlreadyExists", "Já existe um usuário cadastrado com este e-mail."));
        }

        // 3. Instantiate and save User domain profile
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

        // 5. Generate verification token
        user.GenerateVerificationToken();
        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync(cancellationToken);

        // 6. Dispatch welcome/verification email
        await _emailVerificationService.SendVerificationEmailAsync(user.Email, user.VerificationToken!, cancellationToken);

        return user.Id;
    }
}
