using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace MTA.Web.Models;

public class CustomJsonResult<T> : IActionResult
{
    [JsonPropertyName("success")]
    public bool Success { get; init; }

    [JsonPropertyName("data")]
    public T? Data { get; init; }

    [JsonPropertyName("error")]
    public string? Error { get; init; }

    [JsonIgnore]
    public int StatusCode { get; init; } = StatusCodes.Status200OK;

    // ── Factory methods ──────────────────────────────────────────────────────

    public static CustomJsonResult<T> SuccessResult(T data, int statusCode = StatusCodes.Status200OK) => new()
    {
        Success = true,
        Data = data,
        StatusCode = statusCode
    };

    public static CustomJsonResult<T> Created(T data) => new()
    {
        Success = true,
        Data = data,
        StatusCode = StatusCodes.Status201Created
    };

    public static CustomJsonResult<T> NoContent() => new()
    {
        Success = true,
        StatusCode = StatusCodes.Status204NoContent
    };

    public static CustomJsonResult<T> Failure(string error, int statusCode = StatusCodes.Status400BadRequest) => new()
    {
        Success = false,
        Error = error,
        StatusCode = statusCode
    };

    public static CustomJsonResult<T> NotFound(string error = "Resource not found") => new()
    {
        Success = false,
        Error = error,
        StatusCode = StatusCodes.Status404NotFound
    };

    public static CustomJsonResult<T> Unauthorized(string error = "Unauthorized") => new()
    {
        Success = false,
        Error = error,
        StatusCode = StatusCodes.Status401Unauthorized
    };

    public static CustomJsonResult<T> Forbidden(string error = "Forbidden") => new()
    {
        Success = false,
        Error = error,
        StatusCode = StatusCodes.Status403Forbidden
    };

    public static CustomJsonResult<T> Conflict(string error) => new()
    {
        Success = false,
        Error = error,
        StatusCode = StatusCodes.Status409Conflict
    };

    // ── IActionResult ─────────────────────────────────────────────────────────

    public async Task ExecuteResultAsync(ActionContext context)
    {
        var response = context.HttpContext.Response;
        response.StatusCode = StatusCode;

        if (StatusCode != StatusCodes.Status204NoContent)
        {
            response.ContentType = "application/json";
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            await JsonSerializer.SerializeAsync(response.Body, this, options);
        }
    }
}
