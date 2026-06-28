using GreatReports.Application.Common.CQRS;
using GreatReports.Application.Common.Interfaces;
using GreatReports.Domain.Entities;
using GreatReports.Shared;

namespace GreatReports.Application.UseCases.ClientCompanies.Commands;

public record RegisterClientCompanyCommand(Guid ProviderCompanyId, string Name) : ICommand<Guid>;

public class RegisterClientCompanyCommandHandler : ICommandHandler<RegisterClientCompanyCommand, Guid>
{
    private readonly IClientCompanyRepository _clientCompanyRepository;
    private readonly IProviderCompanyRepository _providerCompanyRepository;

    public RegisterClientCompanyCommandHandler(
        IClientCompanyRepository clientCompanyRepository,
        IProviderCompanyRepository providerCompanyRepository)
    {
        _clientCompanyRepository = clientCompanyRepository;
        _providerCompanyRepository = providerCompanyRepository;
    }

    public async Task<Result<Guid>> HandleAsync(RegisterClientCompanyCommand command, CancellationToken cancellationToken = default)
    {
        var providerExists = await _providerCompanyRepository.GetByIdAsync(command.ProviderCompanyId, cancellationToken);
        if (providerExists == null)
        {
            return Result.Failure<Guid>(new Error("ProviderCompany.NotFound", "Provedor não encontrado."));
        }

        var entityResult = ClientCompany.Create(command.ProviderCompanyId, command.Name);
        if (entityResult.IsFailure)
        {
            return Result.Failure<Guid>(entityResult.Error);
        }

        var client = entityResult.Value;
        await _clientCompanyRepository.AddAsync(client, cancellationToken);
        await _clientCompanyRepository.SaveChangesAsync(cancellationToken);

        return client.Id;
    }
}
