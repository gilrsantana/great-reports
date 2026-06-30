using GreatReports.Application.Common.CQRS;
using GreatReports.Application.Common.Models;
using GreatReports.Application.UseCases.ClientCompanies.Commands;
using GreatReports.Application.UseCases.ClientCompanies.Queries;
using GreatReports.Application.UseCases.ClientContacts.Commands;
using GreatReports.Application.UseCases.ClientContacts.Queries;
using GreatReports.Presentation.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GreatReports.Presentation.Controllers;

[Authorize(Roles = "Manager")]
public class ClientCompaniesController(
    ICommandHandler<RegisterClientCompanyCommand, Guid> registerHandler,
    IQueryHandler<GetClientCompaniesQuery, PagedResponse<ClientCompanyDto>> getPagedHandler,
    ICommandHandler<AddClientContactCommand, Guid> addContactHandler,
    IQueryHandler<GetClientContactsQuery, IReadOnlyList<Application.UseCases.ClientContacts.Queries.ClientContactDto>> getContactsHandler) : ApiControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    public async Task<IActionResult> Register([FromBody] RegisterClientCompanyRequest request, CancellationToken cancellationToken)
    {
        var command = new RegisterClientCompanyCommand(request.ProviderCompanyId, request.Name);
        var result = await registerHandler.HandleAsync(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<ClientCompanyDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPaged(
        [FromQuery] Guid providerId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetClientCompaniesQuery(providerId, page, pageSize);
        var result = await getPagedHandler.HandleAsync(query, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("{clientCompanyId:guid}/contacts")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    public async Task<IActionResult> AddContact(
        Guid clientCompanyId,
        [FromBody] AddClientContactRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddClientContactCommand(clientCompanyId, request.Name, request.Email, request.ContactType);
        var result = await addContactHandler.HandleAsync(command, cancellationToken);
        return HandleResult(result);
    }

    [HttpGet("{clientCompanyId:guid}/contacts")]
    [ProducesResponseType(typeof(IReadOnlyList<Application.UseCases.ClientContacts.Queries.ClientContactDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetContacts(Guid clientCompanyId, CancellationToken cancellationToken)
    {
        var query = new GetClientContactsQuery(clientCompanyId);
        var result = await getContactsHandler.HandleAsync(query, cancellationToken);
        return HandleResult(result);
    }
}
