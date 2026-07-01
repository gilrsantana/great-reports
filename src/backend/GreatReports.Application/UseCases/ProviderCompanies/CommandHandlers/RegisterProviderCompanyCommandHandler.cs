using GreatReports.Application.Common.CQRS;
using GreatReports.Application.Common.Interfaces;
using GreatReports.Application.UseCases.ProviderCompanies.Commands;
using GreatReports.Domain.Entities;
using GreatReports.Shared;

namespace GreatReports.Application.UseCases.ProviderCompanies.CommandHandlers;

public class RegisterProviderCompanyCommandHandler(IProviderCompanyRepository providerCompanyRepository, IIdentityService identityService) : ICommandHandler<RegisterProviderCompanyCommand, Guid>
{
    public async Task<Result<Guid>> HandleAsync(RegisterProviderCompanyCommand command, CancellationToken cancellationToken = default)
    {
        if (!await HasManagerRoleAsync(command))
        {
            return Result.Failure<Guid>(new Error("ProviderCompany.UserNotAuthenticated", "Usuário sem permissão."));
        }

        var entityResult = ProviderCompany.Create(command.Name, command.TaxId, command.ManagerId);
        if (entityResult.IsFailure)
        {
            return Result.Failure<Guid>(entityResult.Error);
        }

        var exists = await providerCompanyRepository.ExistsByTaxIdAsync(command.TaxId, cancellationToken);
        if (exists)
        {
            return Result.Failure<Guid>(new Error("ProviderCompany.TaxIdAlreadyExists", "Já existe um provedor cadastrado com este CNPJ/Identificação fiscal."));
        }

        var provider = entityResult.Value;
        await providerCompanyRepository.AddAsync(provider, cancellationToken);
        await providerCompanyRepository.SaveChangesAsync(cancellationToken);

        return provider.Id;
    }

    private async Task<bool> HasManagerRoleAsync(RegisterProviderCompanyCommand command)
    {
        var userRoles = await identityService.GetUserRolesAsync([command.ManagerId]);
        if (userRoles is null)
            return false;

        foreach (var role in userRoles.Values)
        {
            if (role == "Manager")
                return true;
        }

        return false;
    }
}