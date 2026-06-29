using GreatReports.Application.Common.CQRS;
using GreatReports.Application.Common.Models;

namespace GreatReports.Application.UseCases.ClientCompanies.Queries;

public record ClientCompanyDto(Guid Id, string Name);

public record GetClientCompaniesQuery(Guid ProviderId, int Page, int PageSize) : IQuery<PagedResponse<ClientCompanyDto>>;
