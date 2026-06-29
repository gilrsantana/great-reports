using GreatReports.Application.Common.CQRS;
using GreatReports.Application.Common.Interfaces;
using GreatReports.Application.Common.Models;
using GreatReports.Application.UseCases.ClientCompanies.Queries;
using GreatReports.Shared;

namespace GreatReports.Application.UseCases.ClientCompanies.QueryHandlers;

public class GetClientCompaniesQueryHandler : IQueryHandler<GetClientCompaniesQuery, PagedResponse<ClientCompanyDto>>
{
    private readonly IClientCompanyRepository _clientCompanyRepository;

    public GetClientCompaniesQueryHandler(IClientCompanyRepository clientCompanyRepository)
    {
        _clientCompanyRepository = clientCompanyRepository;
    }

    public async Task<Result<PagedResponse<ClientCompanyDto>>> HandleAsync(GetClientCompaniesQuery query, CancellationToken cancellationToken = default)
    {
        if (query.Page <= 0)
        {
            return Result.Failure<PagedResponse<ClientCompanyDto>>(new Error("Query.InvalidPage", "A página deve ser maior que zero."));
        }

        if (query.PageSize <= 0)
        {
            return Result.Failure<PagedResponse<ClientCompanyDto>>(new Error("Query.InvalidPageSize", "O tamanho da página deve ser maior que zero."));
        }

        var (items, totalCount) = await _clientCompanyRepository.GetPagedByProviderIdAsync(
            query.ProviderId,
            query.Page,
            query.PageSize,
            cancellationToken);

        var dtos = items.Select(c => new ClientCompanyDto(c.Id, c.Name)).ToList();

        var response = new PagedResponse<ClientCompanyDto>(dtos, query.Page, query.PageSize, totalCount);
        return response;
    }
}
