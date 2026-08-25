using System.Text.Json;
using FluentValidation;
using MTA.Web.Models;

namespace MTA.Web.Middleware;

/// <summary>
/// Catches unhandled exceptions from controllers/services and maps them to the
/// standard CustomJsonResult envelope with the correct HTTP status code, instead
/// of letting ASP.NET Core return a bare 500 for expected error conditions
/// (bad credentials, validation failures, not-found, conflicts, etc.).
/// </summary>
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
            var (statusCode, message) = Map(ex);

            if (statusCode == StatusCodes.Status500InternalServerError)
                _logger.LogError(ex, "Unhandled exception on {Method} {Path}", context.Request.Method, context.Request.Path);
            else
                _logger.LogWarning(ex, "Handled exception on {Method} {Path}: {Message}", context.Request.Method, context.Request.Path, message);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var payload = new { success = false, data = (object?)null, error = message };
            await context.Response.WriteAsync(JsonSerializer.Serialize(payload));
        }
    }

    private static (int StatusCode, string Message) Map(Exception ex) => ex switch
    {
        UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, ex.Message),
        ValidationException ve => (StatusCodes.Status400BadRequest, string.Join("; ", ve.Errors.Select(e => e.ErrorMessage))),
        ArgumentException => (StatusCodes.Status400BadRequest, ex.Message),
        KeyNotFoundException => (StatusCodes.Status404NotFound, ex.Message),
        InvalidOperationException => (StatusCodes.Status409Conflict, ex.Message),
        _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred. Please try again later.")
    };
}

public static class ExceptionHandlingMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app) =>
        app.UseMiddleware<ExceptionHandlingMiddleware>();
}
