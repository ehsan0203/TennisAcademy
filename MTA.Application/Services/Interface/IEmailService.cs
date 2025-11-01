namespace MTA.Application.Services;

/// <summary>
/// Abstraction for sending transactional emails.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends a password reset email that includes the generated password.
    /// </summary>
    /// <param name="recipientEmail">Destination email address.</param>
    /// <param name="newPassword">Generated password that should be communicated to the user.</param>
    Task SendPasswordResetAsync(string recipientEmail, string newPassword);
}
