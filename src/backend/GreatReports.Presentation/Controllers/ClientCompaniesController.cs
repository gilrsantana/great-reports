using GreatReports.Application.Common.CQRS;
using GreatReports.Application.Common.Models;
using GreatReports.Application.UseCases.ClientCompanies.Commands;
using GreatReports.Application.UseCases.ClientCompanies.Queries;
using GreatReports.Application.UseCases.ClientContacts.Commands;
using GreatReports.Presentation.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreatReports.Presentation.Controllers;

[Authorize(Roles = "Manager")]
public class ClientCompaniesController : ApiControllerBase
{
    private readonly ICommandHandler<RegisterClientCompanyCommand, Guid> _registerHandler;
    private readonly IQueryHandler<GetClientCompaniesQuery, PagedResponse<ClientCompanyDto>> _getPagedHandler;
    private readonly ICommandHandler<AddClientContactCommand, Guid> _addContactHandler;

    public ClientCompaniesController(
        ICommandHandler<RegisterClientCompanyCommand, Guid> registerHandler,
        IQueryHandler<GetClientCompaniesQuery, PagedResponse<ClientCompanyDto>> getPagedHandler,
        ICommandHandler<AddClientContactCommand, Guid> addContactHandler)
    {
        _registerHandler = registerHandler;
        _getPagedHandler = getPagedHandler;
        _addContactHandler = addContactHandler;
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterClientCompanyRequest request, CancellationToken cancellationToken)
    {
        var command = new RegisterClientCompanyCommand(request.ProviderCompanyId, request.Name);
        var result = await _registerHandler.HandleAsync(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged(
        [FromQuery] Guid providerId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetClientCompaniesQuery(providerId, page, pageSize);
        var result = await _getPagedHandler.HandleAsync(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{clientCompanyId:guid}/contacts")]
    public async Task<IActionResult> AddContact(
        Guid clientCompanyId,
        [FromBody] AddClientContactRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddClientContactCommand(clientCompanyId, request.Name, request.Email, request.ContactType);
        var result = await _addContactHandler.HandleAsync(command, cancellationToken);
        return HandleResult(result);
    }
}
