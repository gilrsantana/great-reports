using GreatReports.Application.Common.CQRS;
using GreatReports.Application.Common.Interfaces;
using GreatReports.Application.UseCases.ProviderCompanies.Queries;
using GreatReports.Shared;

namespace GreatReports.Application.UseCases.ProviderCompanies.QueryHandlers;

public class GetProviderDetailsQueryHandler(IProviderCompanyRepository providerCompanyRepository) : IQueryHandler<GetProviderDetailsQuery, ProviderDetailsDto>
{
    public async Task<Result<ProviderDetailsDto>> HandleAsync(GetProviderDetailsQuery query, CancellationToken cancellationToken = default)
    {
        var provider = await providerCompanyRepository.GetByIdAsync(query.ProviderId, cancellationToken);
        if (provider == null)
        {
            return Result.Failure<ProviderDetailsDto>(new Error("ProviderCompany.NotFound", "Provedor não encontrado."));
        }

        var dto = new ProviderDetailsDto(provider.Id, provider.Name, provider.TaxId);
        return dto;
    }
}
