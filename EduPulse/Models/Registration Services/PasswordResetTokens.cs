
namespace EduPulse.Models.Service_Registration
{
    public class PasswordResetTokens
    {

        public int Id { get; set; }
        public int UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
    }
}
