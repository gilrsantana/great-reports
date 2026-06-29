using GreatReports.Application.Common.CQRS;
using GreatReports.Application.Common.Interfaces;
using GreatReports.Application.UseCases.ClientContacts.Commands;
using GreatReports.Domain.Entities;
using GreatReports.Domain.Enums;
using GreatReports.Shared;

namespace GreatReports.Application.UseCases.ClientContacts.CommandHandlers;

public class AddClientContactCommandHandler(
    IClientCompanyRepository clientCompanyRepository,
    IClientContactRepository clientContactRepository,
    IIdentityService identityService) : ICommandHandler<AddClientContactCommand, Guid>
{
    private static readonly string[] _roles = ["Stakeholder"];

    public async Task<Result<Guid>> HandleAsync(AddClientContactCommand command, CancellationToken cancellationToken = default)
    {
        var clientCompany = await clientCompanyRepository.GetByIdAsync(command.ClientCompanyId, cancellationToken);
        if (clientCompany == null)
        {
            return Result.Failure<Guid>(new Error("ClientCompany.NotFound", "Empresa cliente não encontrada."));
        }

        if (!Enum.TryParse<ContactType>(command.ContactType, true, out var type))
        {
            return Result.Failure<Guid>(new Error("ClientContact.InvalidContactType", "O tipo de contato fornecido é inválido. Use Commercial ou Tech."));
        }

        var existingContact = await clientContactRepository.GetByEmailAsync(command.Email, cancellationToken);
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

        await clientContactRepository.AddAsync(contact, cancellationToken);
        await clientContactRepository.SaveChangesAsync(cancellationToken);

        var createAccountSuccess = await identityService.CreateUserAsync(contact.Id, command.Email, GenerateTempPassword(), _roles);
        if (!createAccountSuccess)
        {
            clientContactRepository.Delete(contact);
            await clientContactRepository.SaveChangesAsync(cancellationToken);

            return Result.Failure<Guid>(new Error("ClientContact.RegistrationRollback", "Falha ao criar credenciais de autenticação. O cadastro do contato foi revertido."));
        }

        return contact.Id;
    }

    private static string GenerateTempPassword()
    {
        return Guid.NewGuid().ToString("N") + "aA1!";
    }
}
