using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using MTA.Application.Services.Interface;

namespace MTA.Application.Services.Service;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly string _smtpUser;
    private readonly string _smtpPass;
    private readonly string _fromEmail;
    private readonly string _fromName;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _logger = logger;

        _smtpHost = config["Email:SmtpHost"] ?? string.Empty;
        _smtpPort = int.TryParse(config["Email:SmtpPort"], out var port) ? port : 587;
        _smtpUser = config["Email:SmtpUser"] ?? string.Empty;
        _smtpPass = config["Email:SmtpPass"] ?? string.Empty;
        _fromEmail = config["Email:FromEmail"] ?? string.Empty;
        _fromName = config["Email:FromName"] ?? "MTA Tennis Academy";
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlBody, CancellationToken ct = default)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_fromName, _fromEmail));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = subject;
        message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(_smtpHost, _smtpPort, SecureSocketOptions.StartTls, ct);
            await client.AuthenticateAsync(_smtpUser, _smtpPass, ct);
            await client.SendAsync(message, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {ToEmail}", toEmail);
            throw;
        }
        finally
        {
            if (client.IsConnected)
                await client.DisconnectAsync(true, ct);
        }
    }
}
