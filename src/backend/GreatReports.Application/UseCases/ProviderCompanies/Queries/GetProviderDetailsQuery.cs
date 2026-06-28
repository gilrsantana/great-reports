using GreatReports.Application.Common.CQRS;
using GreatReports.Application.Common.Interfaces;
using GreatReports.Shared;

namespace GreatReports.Application.UseCases.ProviderCompanies.Queries;

public record ProviderDetailsDto(Guid Id, string Name, string TaxId);

public record GetProviderDetailsQuery(Guid ProviderId) : IQuery<ProviderDetailsDto>;

public class GetProviderDetailsQueryHandler : IQueryHandler<GetProviderDetailsQuery, ProviderDetailsDto>
{
    private readonly IProviderCompanyRepository _providerCompanyRepository;

    public GetProviderDetailsQueryHandler(IProviderCompanyRepository providerCompanyRepository)
    {
        _providerCompanyRepository = providerCompanyRepository;
    }

    public async Task<Result<ProviderDetailsDto>> HandleAsync(GetProviderDetailsQuery query, CancellationToken cancellationToken = default)
    {
        var provider = await _providerCompanyRepository.GetByIdAsync(query.ProviderId, cancellationToken);
        if (provider == null)
        {
            return Result.Failure<ProviderDetailsDto>(new Error("ProviderCompany.NotFound", "Provedor não encontrado."));
        }

        var dto = new ProviderDetailsDto(provider.Id, provider.Name, provider.TaxId);
        return dto;
    }
}
