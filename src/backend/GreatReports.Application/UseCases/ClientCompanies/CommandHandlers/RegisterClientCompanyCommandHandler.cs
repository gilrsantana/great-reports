using GreatReports.Application.Common.CQRS;
using GreatReports.Application.Common.Interfaces;
using GreatReports.Application.UseCases.ClientCompanies.Commands;
using GreatReports.Domain.Entities;
using GreatReports.Shared;

namespace GreatReports.Application.UseCases.ClientCompanies.CommandHandlers;

public class RegisterClientCompanyCommandHandler(
    IClientCompanyRepository clientCompanyRepository,
    IProviderCompanyRepository providerCompanyRepository) : ICommandHandler<RegisterClientCompanyCommand, Guid>
{
    public async Task<Result<Guid>> HandleAsync(RegisterClientCompanyCommand command, CancellationToken cancellationToken = default)
    {
        var providerExists = await providerCompanyRepository.GetByIdAsync(command.ProviderCompanyId, cancellationToken);
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
        await clientCompanyRepository.AddAsync(client, cancellationToken);
        await clientCompanyRepository.SaveChangesAsync(cancellationToken);

        return client.Id;
    }
}
