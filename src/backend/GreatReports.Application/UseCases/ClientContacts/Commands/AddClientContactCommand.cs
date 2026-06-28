using GreatReports.Application.Common.CQRS;
using GreatReports.Application.Common.Interfaces;
using GreatReports.Domain.Entities;
using GreatReports.Domain.Enums;
using GreatReports.Shared;

namespace GreatReports.Application.UseCases.ClientContacts.Commands;

public record AddClientContactCommand(Guid ClientCompanyId, string Name, string Email, string ContactType) : ICommand<Guid>;

public class AddClientContactCommandHandler : ICommandHandler<AddClientContactCommand, Guid>
{
    private readonly IClientCompanyRepository _clientCompanyRepository;
    private readonly IEmailVerificationService _emailVerificationService;

    // We can inject IRepository<ClientContact> or IUserRepository to check for existing email,
    // but the spec doesn't specify check, but we should make sure we have a way to save it.
    // Wait, is there a client contact repository? Yes, we can just inject a generic repository,
    // or add ClientContact directly to DbContext if we have it, or define IClientContactRepository.
    // Wait, let's look at what repositories are listed under Interfaces:
    // - IProviderCompanyRepository
    // - IClientCompanyRepository
    // - IProjectRepository
    // - IUserRepository
    // - IGroupRepository
    // - IDailyActivityRepository
    // Ah! It doesn't list IClientContactRepository!
    // But wait, under Infrastructure mappings we have:
    // `ClientContactConfiguration.cs` (table "ClientContacts", unique Email, delete behavior restrict)
    // How do we save/add a ClientContact?
    // In Clean Architecture, we can define `IClientContactRepository` or just inject `GreatReportsDbContext` inside a repository or define a generic repository, or add `IClientContactRepository` to interfaces!
    // Let's add `IClientContactRepository.cs` to Application interfaces and register it! That's much cleaner than directly using DbContext in the handler.
    // Wait, let's first check if we can add IClientContactRepository:
    // Yes! Let's write `IClientContactRepository.cs` and use it.

    private readonly IClientContactRepository _clientContactRepository;

    public AddClientContactCommandHandler(
        IClientCompanyRepository clientCompanyRepository,
        IClientContactRepository clientContactRepository,
        IEmailVerificationService emailVerificationService)
    {
        _clientCompanyRepository = clientCompanyRepository;
        _clientContactRepository = clientContactRepository;
        _emailVerificationService = emailVerificationService;
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

        var entityResult = ClientContact.Create(command.ClientCompanyId, command.Name, command.Email, type);
        if (entityResult.IsFailure)
        {
            return Result.Failure<Guid>(entityResult.Error);
        }

        var contact = entityResult.Value;
        contact.GenerateVerificationToken();

        await _clientContactRepository.AddAsync(contact, cancellationToken);
        await _clientContactRepository.SaveChangesAsync(cancellationToken);

        // Dispatch verification email
        await _emailVerificationService.SendVerificationEmailAsync(contact.Email, contact.VerificationToken!, cancellationToken);

        return contact.Id;
    }
}
