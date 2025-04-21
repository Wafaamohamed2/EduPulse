using EduPulse.Models.Services;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

public class EmailService : IEmailService
{
    private readonly string _smtpServer = "smtp.gmail.com";
    private readonly int _smtpPort = 587; // Use 465 for SSL or 587 for TLS
    private readonly string _emailAddress = "mohamedwafaa245@gmail.com"; // Your Gmail address
    private readonly string _emailPassword = "vdiq turm ungx dhiy"; // The App Password you generated in Gmail


    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
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
            smtpClient.EnableSsl = true;

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                // Handle errors
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }
    }
}
