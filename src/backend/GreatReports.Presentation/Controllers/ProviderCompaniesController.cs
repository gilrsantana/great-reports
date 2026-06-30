using GreatReports.Application.Common.CQRS;
using GreatReports.Application.UseCases.ProviderCompanies.Commands;
using GreatReports.Application.UseCases.ProviderCompanies.Queries;
using GreatReports.Presentation.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreatReports.Presentation.Controllers;

[Authorize(Roles = "Manager")]
public class ProviderCompaniesController : ApiControllerBase
{
    private readonly ICommandHandler<RegisterProviderCompanyCommand, Guid> _registerHandler;
    private readonly IQueryHandler<GetProviderDetailsQuery, ProviderDetailsDto> _detailsHandler;

    public ProviderCompaniesController(
        ICommandHandler<RegisterProviderCompanyCommand, Guid> registerHandler,
        IQueryHandler<GetProviderDetailsQuery, ProviderDetailsDto> detailsHandler)
    {
        _registerHandler = registerHandler;
        _detailsHandler = detailsHandler;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    public async Task<IActionResult> Register([FromBody] RegisterProviderCompanyRequest request, CancellationToken cancellationToken)
    {
        var command = new RegisterProviderCompanyCommand(request.Name, request.TaxId);
        var result = await _registerHandler.HandleAsync(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProviderDetailsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDetails(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetProviderDetailsQuery(id);
        var result = await _detailsHandler.HandleAsync(query, cancellationToken);
        return HandleResult(result);
    }
}
