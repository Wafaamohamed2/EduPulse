namespace EduPulse.Models.Services
{
    public class EmailServiceFactory : IEmailServiceFactory
    {
        private readonly IConfiguration _configuration;

        public EmailServiceFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IEmailService CreateEmailService()
        {
            var useSmtp = _configuration.GetValue<bool>("EmailSettings:UseSmtp");

            if (useSmtp)
            {
                return new EmailService();  // Stmp
            }
            else
            {
                throw new NotImplementedException("Other email services are not yet implemented.");
            }
        }
    }

}
