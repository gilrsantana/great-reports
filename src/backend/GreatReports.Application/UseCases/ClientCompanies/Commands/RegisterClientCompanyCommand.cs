using GreatReports.Application.Common.CQRS;

namespace GreatReports.Application.UseCases.ClientCompanies.Commands;

public record RegisterClientCompanyCommand(Guid ProviderCompanyId, string Name) : ICommand<Guid>;
