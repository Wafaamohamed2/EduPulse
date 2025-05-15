namespace EduPulse.Models.Services
{
    public class EmailServiceFactory : IEmailServiceFactory
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        public EmailServiceFactory(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public IEmailService CreateEmailService()
        {
            var useSmtp = _configuration.GetValue<bool>("EmailSettings:UseSmtp");

            if (useSmtp)
            {
                return new EmailService(_configuration, _logger);
            }
            else
            {
                throw new NotImplementedException("Other email services are not yet implemented.");
            }
        }
    }

}
