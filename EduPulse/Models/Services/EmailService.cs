using EduPulse.Models.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

public class EmailService : IEmailService
{
    private readonly string _smtpServer;
    private readonly int _smtpPort;
    private readonly string _emailAddress;
    private readonly string _emailPassword;
    private readonly bool _enableSsl;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _smtpServer = configuration["SmtpSettings:Host"] ?? "smtp.gmail.com";
        _smtpPort = int.Parse(configuration["SmtpSettings:Port"] ?? "587");
        _emailAddress = configuration["SmtpSettings:Username"] ?? "mohamedwafaa245@gmail.com";
        _emailPassword = configuration["SmtpSettings:Password"] ?? "vdiq turm ungx dhiy";
        _enableSsl = bool.Parse(configuration["SmtpSettings:EnableSsl"] ?? "true");
        _logger = logger;

        _logger.LogInformation("Email service initialized with server: {Server}, Port: {Port}, Username: {Username}", 
            _smtpServer, _smtpPort, _emailAddress);
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        _logger.LogInformation("Attempting to send email to {Recipient} with subject: {Subject}", toEmail, subject);

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_emailAddress),
            Subject = subject,
            Body = body,
            IsBodyHtml = true,
        };

        mailMessage.To.Add(toEmail);

        using (var smtpClient = new SmtpClient(_smtpServer))
        {
            smtpClient.Port = _smtpPort;
            smtpClient.Credentials = new NetworkCredential(_emailAddress, _emailPassword);
            smtpClient.EnableSsl = _enableSsl;

            try
            {
                _logger.LogInformation("Sending email to {Recipient}...", toEmail);
                await smtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation("Email successfully sent to {Recipient}", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Recipient}: {ErrorMessage}", toEmail, ex.Message);
                
                // Rethrow exception to ensure caller knows email wasn't sent
                throw new Exception($"Failed to send email: {ex.Message}", ex);
            }
        }
    }
}
