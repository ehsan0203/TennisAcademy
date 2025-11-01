using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MTA.Application.Services;
using MTA.Infrastructure.Options;

namespace MTA.Infrastructure.Services;

/// <summary>
/// Sends transactional emails through the Mailgun REST API.
/// </summary>
public class MailgunEmailService : IEmailService
{
    private readonly HttpClient _httpClient;
    private readonly MailgunOptions _options;
    private readonly ILogger<MailgunEmailService> _logger;

    public MailgunEmailService(HttpClient httpClient, IOptions<MailgunOptions> options, ILogger<MailgunEmailService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task SendPasswordResetAsync(string recipientEmail, string newPassword)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey) || string.IsNullOrWhiteSpace(_options.Domain) || string.IsNullOrWhiteSpace(_options.From))
        {
            throw new InvalidOperationException("Mailgun configuration is incomplete.");
        }

        var requestUri = $"{_options.BaseUrl.TrimEnd('/')}/v3/{_options.Domain}/messages";
        using var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
        {
            Content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["from"] = _options.From,
                ["to"] = recipientEmail,
                ["subject"] = "Password reset request",
                ["text"] = $"Your new password is: {newPassword}\nPlease change it after logging in."
            })
        };

        var credentials = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"api:{_options.ApiKey}"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            _logger.LogError("Failed to send Mailgun email. StatusCode: {StatusCode}, Response: {Response}", response.StatusCode, responseBody);
            throw new InvalidOperationException("Failed to send password reset email.");
        }
    }
}
