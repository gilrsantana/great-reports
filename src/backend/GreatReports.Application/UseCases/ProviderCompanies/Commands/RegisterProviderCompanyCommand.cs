using GreatReports.Application.Common.CQRS;

namespace GreatReports.Application.UseCases.ProviderCompanies.Commands;

public record RegisterProviderCompanyCommand(string Name, string TaxId) : ICommand<Guid>;
