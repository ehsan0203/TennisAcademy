namespace MTA.Infrastructure.Options;

/// <summary>
/// Configuration options required to connect to the Mailgun API.
/// </summary>
public class MailgunOptions
{
    /// <summary>
    /// Base URL of the Mailgun API. Defaults to the public endpoint.
    /// </summary>
    public string BaseUrl { get; set; } = "https://api.mailgun.net";

    /// <summary>
    /// Domain configured in Mailgun.
    /// </summary>
    public string Domain { get; set; } = string.Empty;

    /// <summary>
    /// API key provided by Mailgun.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Sender email address used for password reset emails.
    /// </summary>
    public string From { get; set; } = string.Empty;
}
