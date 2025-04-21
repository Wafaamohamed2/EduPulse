using EduPulse.Models.DTOs;
using EduPulse.Models.Services;
using EduPulse.Models.Users;
using Microsoft.EntityFrameworkCore;
using SW_Project.Models;

namespace EduPulse.Models.Service_Registration
{
    public class TeacherRegistrationService
    {

        private readonly SW_Entity _context;
        private readonly NotificationService _notificationService;
        public TeacherRegistrationService(SW_Entity context, NotificationService notification)
        {
            _context = context;
            _notificationService = notification;
        }

        public async Task<Teacher> RegisterAsync(TeacherRegisterDto request)
        {
            if (await _context.Teachers.AnyAsync(t => t.Email == request.Email))
                throw new Exception("Email already exists");
            if (request.Name == null || request.Email == null)
            {
                
                throw new ArgumentNullException("Name or Email cannot be null");
            }
            var teacher = new Teacher
            {
                Name = request.Name,
                Email = request.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Confirm_Password = BCrypt.Net.BCrypt.HashPassword(request.Confirm_Password),
                Subject = request.Subject
            };

            
            await _context.Teachers.AddAsync(teacher);
            await _context.SaveChangesAsync();

            return teacher;
        }
    }
}