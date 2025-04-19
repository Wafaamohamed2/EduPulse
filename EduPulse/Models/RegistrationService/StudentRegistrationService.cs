using EduPulse.Models.DTOs;
using EduPulse.Models.Users;
using Microsoft.EntityFrameworkCore;
using SW_Project.Models;

namespace EduPulse.Models.Service_Registration
{
    public class StudentRegistrationService
    {

        private readonly SW_Entity _context;

        public StudentRegistrationService(SW_Entity context)
        {
            _context = context;
        }

        public async Task<Student> RegisterAsync(StudentRegisterDto request)
        {
            if (await _context.Students.AnyAsync(s => s.Email == request.Email))
                throw new Exception("Email already exists");

            var student = new Student
            {
                Name = request.Name,
                Email = request.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Confirm_Password = BCrypt.Net.BCrypt.HashPassword(request.ConfirmPassword),
                Level = request.Level,
                FingerId = request.FingerId
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();


            return student;
        }
    }
}
