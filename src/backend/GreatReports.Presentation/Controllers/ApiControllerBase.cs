using GreatReports.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

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
        var code = result.Error.Code;
        var status = StatusCodes.Status400BadRequest;
        
        if (code == "Auth.InvalidCredentials" || code == "Auth.InvalidToken" || 
            code == "Token.Expired" || code == "Auth.InvalidRefreshToken" || 
            code == "Auth.EmailNotConfirmed" || code == "Auth.AccountLocked")
        {
            status = StatusCodes.Status401Unauthorized;
        }
        else if (code.Contains("NotFound"))
        {
            status = StatusCodes.Status404NotFound;
        }
        
        var problemDetails = new ProblemDetails
        {
            Title = status == StatusCodes.Status401Unauthorized ? "Não Autorizado" :
                    status == StatusCodes.Status404NotFound ? "Não Encontrado" : "Erro na Requisição",
            Status = status,
            Detail = result.Error.Description,
            Type = status == StatusCodes.Status401Unauthorized ? "https://tools.ietf.org/html/rfc7235#section-3.1" :
                   status == StatusCodes.Status404NotFound ? "https://tools.ietf.org/html/rfc7231#section-6.5.4" :
                   "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Extensions = { { "code", code } }
        };

        if (result.Error is ValidationError validationError)
        {
            problemDetails.Title = "Erro de Validação";
            problemDetails.Status = StatusCodes.Status400BadRequest;
            problemDetails.Detail = validationError.Description;
            problemDetails.Extensions.Add("errors", validationError.Errors);
            return BadRequest(problemDetails);
        }

        return status switch
        {
            StatusCodes.Status401Unauthorized => Unauthorized(problemDetails),
            StatusCodes.Status404NotFound => NotFound(problemDetails),
            _ => BadRequest(problemDetails)
        };
    }
}
