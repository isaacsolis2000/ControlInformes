using System.Net;
using System.Text.Json;
using ControlInformes.Utils;

namespace ControlInformes.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "Error no controlado: {Message}", exception.Message);

        var response = ApiResponse<object>.Error(
            ErrorCatalog.GetMensaje(ErrorCatalog.ErrorInterno),
            ErrorCatalog.ErrorInterno);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = response.HttpCode;

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
}
