using GreatReports.Application.Common.CQRS;

namespace GreatReports.Application.UseCases.ProviderCompanies.Queries;

public record ProviderDetailsDto(Guid Id, string Name, string TaxId);

public record GetProviderDetailsQuery(Guid ProviderId) : IQuery<ProviderDetailsDto>;
