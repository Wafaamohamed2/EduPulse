using EduPulse.Models.DTOs;
using EduPulse.Models.Users;
using Microsoft.EntityFrameworkCore;
using SW_Project.Models;

namespace EduPulse.Models.Service_Registration
{
    public class TeacherRegistrationService
    {

        private readonly SW_Entity _context;

        public TeacherRegistrationService(SW_Entity context)
        {
            _context = context;
        }

        public async Task<Teacher> RegisterAsync(TeacherRegisterDto request)
        {
            if (await _context.Teachers.AnyAsync(t => t.Email == request.Email))
                throw new Exception("Email already exists");

            var teacher = new Teacher
            {
                Name = request.Name,
                Email = request.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Confirm_Password = BCrypt.Net.BCrypt.HashPassword(request.ConfirmPassword),
                Subject = request.Subject
            };

            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();

            return teacher;
        }
    }
}
