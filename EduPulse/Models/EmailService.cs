//using MailKit.Net.Smtp;
//using MimeKit;
//using System.Threading.Tasks;

//namespace StudentAttendanceAPI.Services
//{
//    public class EmailService
//    {
//        private readonly string _smtpServer;
//        private readonly int _port;
//        private readonly string _username;
//        private readonly string _password;

//        public EmailService(string smtpServer, int port, string username, string password)
//        {
//            _smtpServer = smtpServer;
//            _port = port;
//            _username = username;
//            _password = password;
//        }

//        public async Task SendEmailAsync(string toEmail, string subject, string message)
//        {
//            var emailMessage = new MimeMessage();
//            emailMessage.From.Add(new MailboxAddress("Your Name", _username));
//            emailMessage.To.Add(new MailboxAddress("", toEmail));
//            emailMessage.Subject = subject;
//            emailMessage.Body = new TextPart("plain") { Text = message };

//            using (var client = new SmtpClient())
//            {
//                await client.ConnectAsync(_smtpServer, _port, true);
//                await client.AuthenticateAsync(_username, _password);
//                await client.SendAsync(emailMessage);
//                await client.DisconnectAsync(true);
//            }
//        }
//    }
//}
