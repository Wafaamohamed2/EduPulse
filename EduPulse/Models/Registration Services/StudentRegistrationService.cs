using EduPulse.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using EduPulse.Models.Users;
using SW_Project.Models;
using EduPulse.Models.Services;



namespace EduPulse.Models.Service_Registration
{
    public class StudentRegistrationService
    {

        private readonly SW_Entity _context;
        private readonly NotificationService _notificationService;
        

        public StudentRegistrationService(SW_Entity context , NotificationService notification)
        {
            _context = context;
            _notificationService = notification;
        }

        public async Task<Student> RegisterAsync(StudentRegisterDto request)
        {
            if (await _context.Students.AnyAsync(s => s.Email == request.Email))
                throw new Exception("Email already exists");
            if (request.Name == null || request.Email == null)
            {
                
                throw new ArgumentNullException("Name or Email cannot be null");
            }
            var student = new Student
            {
                Name = request.Name,
                Email = request.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Confirm_Password = BCrypt.Net.BCrypt.HashPassword(request.Confirm_Password),
                FingerId = request.FingerId
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            



            return student;
        }
    }
}
