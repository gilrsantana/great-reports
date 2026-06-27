using GreatReports.Shared;
using Microsoft.AspNetCore.Mvc;

namespace GreatReports.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    protected ActionResult HandleResult(Result result)
    {
        if (result.IsSuccess)
        {
            return NoContent();
        }

        return MapFailure(result);
    }

    protected ActionResult HandleResult<TValue>(Result<TValue> result)
    {
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return MapFailure(result);
    }

    private ActionResult MapFailure(Result result)
    {
        if (result.Error is ValidationError validationError)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Erro de Validação",
                Status = StatusCodes.Status400BadRequest,
                Detail = validationError.Description,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            };

            problemDetails.Extensions.Add("errors", validationError.Errors);

            return BadRequest(problemDetails);
        }

        return BadRequest(new ProblemDetails
        {
            Title = "Erro na Requisição",
            Status = StatusCodes.Status400BadRequest,
            Detail = result.Error.Description,
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Extensions = { { "code", result.Error.Code } }
        });
    }
}
