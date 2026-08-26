namespace MTA.Application.Services.Interface;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string htmlBody, CancellationToken ct = default);
}
