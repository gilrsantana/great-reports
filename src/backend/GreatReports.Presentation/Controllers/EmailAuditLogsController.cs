using GreatReports.Application.Common.CQRS;
using GreatReports.Application.Common.Models;
using GreatReports.Application.UseCases.EmailAuditLogs.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GreatReports.Presentation.Controllers;

[Authorize(Roles = "Maintainer,Manager")]
public class EmailAuditLogsController(
    IQueryHandler<GetEmailAuditLogsQuery, PagedResponse<EmailAuditLogDto>> queryHandler) : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<EmailAuditLogDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromQuery] string? recipient, [FromQuery] bool? success, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = new GetEmailAuditLogsQuery(recipient, success, page, pageSize);
        var result = await queryHandler.HandleAsync(query, cancellationToken);
        return HandleResult(result);
    }
}
