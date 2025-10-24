using System.Text.Json.Serialization;

namespace MTA.Web.Models;

public class CustomJsonResult<T>
{
    [JsonPropertyName("success")]
    public bool Success { get; init; }

    [JsonPropertyName("data")]
    public T? Data { get; init; }

    [JsonPropertyName("error")]
    public string? Error { get; init; }

    public static CustomJsonResult<T> SuccessResult(T data) => new()
    {
        Success = true,
        Data = data
    };

    public static CustomJsonResult<T> Failure(string error) => new()
    {
        Success = false,
        Error = error
    };
}
