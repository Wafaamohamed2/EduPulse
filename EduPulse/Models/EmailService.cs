using MailKit.Net.Smtp;
using MimeKit;
using System.Threading.Tasks;

namespace StudentAttendanceAPI.Services
{
    public class EmailService
    {
        
        private readonly string _smtpServer;
        private readonly int _port;
        private readonly string _username;
        private readonly string _password;


        // Private constructor to prevent direct instantiation

        public EmailService(IConfiguration configuration)
        {
            _smtpServer = configuration["Email:SmtpServer"];
            _port = int.Parse(configuration["Email:Port"]);
            _username = configuration["Email:Username"];
            _password = configuration["Email:Password"];
        }

       


        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Your Name", _username));
            emailMessage.To.Add(new MailboxAddress("", toEmail));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("plain") { Text = message };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_smtpServer, _port, true);
                await client.AuthenticateAsync(_username, _password);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }
    }
}
