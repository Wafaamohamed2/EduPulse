using EduPulse.Models.DTOs;
using EduPulse.Models.Services;
using EduPulse.Models.Users;
using Microsoft.EntityFrameworkCore;
using SW_Project.Models;

namespace EduPulse.Models.Service_Registration
{
    public class ParentRegistrationService
    {
        private readonly SW_Entity _context;
        private readonly NotificationService _notificationService;
        public ParentRegistrationService(SW_Entity context , NotificationService notification)
        {
            _context = context;
           _notificationService = notification;
        }

        public async Task<Parent> RegisterAsync(ParentRegisterDto request)
        {
            if (await _context.Parents.AnyAsync(p => p.Email == request.Email))
                throw new Exception("Email already exists");

            var student = await _context.Students.FindAsync(request.StudentId);
            if (student == null)
            {
                throw new Exception("Student not found");
            }
            if (request.Name == null || request.Email == null)
            {
                
                throw new ArgumentNullException("Name or Email cannot be null");
            }
            var parent = new Parent
            {
                Name = request.Name,
                Email = request.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Confirm_Password = BCrypt.Net.BCrypt.HashPassword(request.ConfirmPassword),
                studentId = request.StudentId
            };
            _context.Parents.Add(parent);
            await _context.SaveChangesAsync();

          

            return parent;
        }
    }
}