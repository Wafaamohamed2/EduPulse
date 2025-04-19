using EduPulse.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StudentAttendanceAPI.Services;
using SW_Project.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EduPulse.Models
{
    public class AuthService
    {
        private readonly SW_Entity _context;
        private readonly IConfiguration _config;
        private readonly EmailService _emailService;

        public AuthService(SW_Entity context, IConfiguration config, EmailService emailService)
        {
            _context = context;
            _config = config;
            _emailService = emailService;
        }

        private string GenerateJwtToken(IUser user)
        {
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.GetType().Name)
    };

            // Add student ID claim for parents
            if (user is Parent parent)
            {
                claims.Add(new Claim("StudentId", parent.studentId.ToString()));
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<IUser> Authenticate(string email, string password)
        {
            // Check in all user types
            var student = await _context.Students.FirstOrDefaultAsync(u => u.Email == email);
            if (student != null && student.Password == password) return (IUser)student;

            var teacher = await _context.Teachers.FirstOrDefaultAsync(u => u.Email == email);
            if (teacher != null && teacher.Password == password) return (IUser)teacher;

            var parent = await _context.Parents.FirstOrDefaultAsync(u => u.Email == email);
            if (parent != null && parent.Password == password) return (IUser)parent;

            return null;
        }

        public async Task<bool> SendPasswordResetEmail(string email)
        {
            var user = await _context.Students.FirstOrDefaultAsync(u => u.Email == email) as IUser ??
                       await _context.Teachers.FirstOrDefaultAsync(u => u.Email == email) as IUser ??
                       await _context.Parents.FirstOrDefaultAsync(u => u.Email == email) as IUser;

            if (user == null) return false;

            var token = GeneratePasswordResetToken(user);
            var resetLink = $"https://yourapp.com/reset-password?token={token}&email={user.Email}";

            await _emailService.SendEmailAsync(
                user.Email,
                "Password Reset Request",
                $"Please reset your password using this link: {resetLink}");

            return true;
        }

        private string GeneratePasswordResetToken(IUser user)
        {
            return Guid.NewGuid().ToString();
        }
    }
    public interface IUser
    {
        int Id { get; set; }
        string Email { get; set; }
        string Password { get; set; }
        string Name { get; set; }
    }
}
