namespace EduPulse.Models.Users
{
   public interface IUser
    {
        int Id { get; set; }
        string Name { get; set; }
        string Email { get; set; }
        string Password { get; set; }
        string PhoneNumber { get; set; }
        string ProfilePictureUrl { get; set; }

        bool Login(string email, string password);
        bool SignUp(string name, string email, string password);
    }
}
