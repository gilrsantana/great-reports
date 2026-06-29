using GreatReports.Application.Common.CQRS;
using GreatReports.Application.Common.Interfaces;
using GreatReports.Application.UseCases.ClientContacts.Commands;
using GreatReports.Domain.Entities;
using GreatReports.Domain.Enums;
using GreatReports.Shared;

namespace GreatReports.Application.UseCases.ClientContacts.CommandHandlers;

public class AddClientContactCommandHandler : ICommandHandler<AddClientContactCommand, Guid>
{
    private static readonly string[] _roles = { "Stakeholder" };
    private readonly IClientCompanyRepository _clientCompanyRepository;
    private readonly IClientContactRepository _clientContactRepository;
    private readonly IIdentityService _identityService;

    public AddClientContactCommandHandler(
        IClientCompanyRepository clientCompanyRepository,
        IClientContactRepository clientContactRepository,
        IIdentityService identityService)
    {
        _clientCompanyRepository = clientCompanyRepository;
        _clientContactRepository = clientContactRepository;
        _identityService = identityService;
    }

    public async Task<Result<Guid>> HandleAsync(AddClientContactCommand command, CancellationToken cancellationToken = default)
    {
        var clientCompany = await _clientCompanyRepository.GetByIdAsync(command.ClientCompanyId, cancellationToken);
        if (clientCompany == null)
        {
            return Result.Failure<Guid>(new Error("ClientCompany.NotFound", "Empresa cliente não encontrada."));
        }

        if (!Enum.TryParse<ContactType>(command.ContactType, true, out var type))
        {
            return Result.Failure<Guid>(new Error("ClientContact.InvalidContactType", "O tipo de contato fornecido é inválido. Use Commercial ou Tech."));
        }

        var existingContact = await _clientContactRepository.GetByEmailAsync(command.Email, cancellationToken);
        if (existingContact != null)
        {
            return Result.Failure<Guid>(new Error("ClientContact.EmailAlreadyExists", "Já existe um contato cadastrado com este e-mail."));
        }

        var entityResult = ClientContact.Create(command.ClientCompanyId, command.Name, command.Email, type);
        if (entityResult.IsFailure)
        {
            return Result.Failure<Guid>(entityResult.Error);
        }

        var contact = entityResult.Value;

        await _clientContactRepository.AddAsync(contact, cancellationToken);
        await _clientContactRepository.SaveChangesAsync(cancellationToken);

        var createAccountSuccess = await _identityService.CreateUserAsync(contact.Id, command.Email, GenerateTempPassword(), _roles);
        if (!createAccountSuccess)
        {
            _clientContactRepository.Delete(contact);
            await _clientContactRepository.SaveChangesAsync(cancellationToken);

            return Result.Failure<Guid>(new Error("ClientContact.RegistrationRollback", "Falha ao criar credenciais de autenticação. O cadastro do contato foi revertido."));
        }

        return contact.Id;
    }

    private static string GenerateTempPassword()
    {
        return Guid.NewGuid().ToString("N") + "aA1!";
    }
}
