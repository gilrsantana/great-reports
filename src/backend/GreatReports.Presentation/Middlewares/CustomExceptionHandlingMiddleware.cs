using Microsoft.AspNetCore.Mvc;

namespace GreatReports.Presentation.Middlewares;

public class CustomExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CustomExceptionHandlingMiddleware> _logger;

    public CustomExceptionHandlingMiddleware(RequestDelegate next, ILogger<CustomExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ocorreu uma exceção não tratada durante o processamento da requisição.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var problemDetails = new ProblemDetails
        {
            Title = "Erro Interno do Servidor",
            Status = StatusCodes.Status500InternalServerError,
            Detail = "Ocorreu um erro inesperado no servidor.",
            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1"
        };

        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}
