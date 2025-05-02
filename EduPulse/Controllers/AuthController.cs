using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EduPulse.Models.DTOs;
using SW_Project.Models;
using EduPulse.Models.Service_Registration;
using EduPulse.Models.Services;

namespace EduPulse.Models.Users
{
    [ApiController]
    [Route("register")]
    public class AuthController : Controller
    {
        private readonly SW_Entity _context;
        private readonly IConfiguration _configuration;
        private readonly NotificationService _notificationService;

        // Registration Services
        private readonly StudentRegistrationService _studentRegistrationService;
        private readonly TeacherRegistrationService _teacherRegistrationService;
        private readonly ParentRegistrationService _parentRegistrationService;

        public AuthController(SW_Entity context, IConfiguration configuration, NotificationService notificationService)
        {
            _context = context;
            _configuration = configuration;
            _notificationService = notificationService;

            _studentRegistrationService = new StudentRegistrationService(context, notificationService);
            _teacherRegistrationService = new TeacherRegistrationService(context, notificationService);
            _parentRegistrationService = new ParentRegistrationService(context, notificationService);
        }

        // Verify Password
        private bool VerifyPassword(string inputPassword, string storedPassword)
        {
            if (string.IsNullOrEmpty(inputPassword) || string.IsNullOrEmpty(storedPassword))
                return false;
            return BCrypt.Net.BCrypt.Verify(inputPassword, storedPassword);
        }

        // Hash Password
        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // Login
        #region
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _context.Students.FirstOrDefaultAsync(u => u.Email == loginDto.Email) as IUser
                    ?? await _context.Teachers.FirstOrDefaultAsync(u => u.Email == loginDto.Email) as IUser
                    ?? await _context.Parents.FirstOrDefaultAsync(u => u.Email == loginDto.Email) as IUser;

            if (user == null || string.IsNullOrWhiteSpace(loginDto.Password) || !VerifyPassword(loginDto.Password, user.Password))
                return Unauthorized("Invalid Information");

            // Use the Singleton JwtTokenGenerator
            var token = JwtTokenGenerator.GetInstance(_configuration).GenerateJwtToken(user);

            return Ok(new { Token = token, UserType = user.GetType().Name });
        }
        #endregion


        // Student Register
        #region
        [HttpPost("student")]
        public async Task<IActionResult> RegisterStudent([FromBody] StudentRegisterDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var student = await _studentRegistrationService.RegisterAsync(request);

                // Use the Singleton JwtTokenGenerator
                var token = JwtTokenGenerator.GetInstance(_configuration).GenerateJwtToken(student);

                var response = new
                {
                    Token = token,
                    UserType = "Student",
                    UserId = student.Id,
                    Name = student.Name,
                    Email = student.Email
                };

                try
                {
                    await _notificationService.NotifyStudentRegistered(student.Email, student.Name);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Email sending failed: {ex.Message}");
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine("⚠️ Error in RegisterStudent:\n" + ex);
                return BadRequest(new { Message = ex.Message });
            }
        }
        #endregion


        // Teacher Register
        #region
        [HttpPost("register/teacher")]
        public async Task<IActionResult> RegisterTeacher([FromBody] TeacherRegisterDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var teacher = await _teacherRegistrationService.RegisterAsync(request);

                // Use the Singleton JwtTokenGenerator
                var token = JwtTokenGenerator.GetInstance(_configuration).GenerateJwtToken(teacher);
                Console.WriteLine("✅ Preparing to notify teacher...");

                await _notificationService.NotifyTeacherRegistered(teacher.Email, teacher.Name, teacher.Subject);

                Console.WriteLine("✅ Email notification sent.");

                return CreatedAtAction(nameof(Login), new
                {
                    Token = token,
                    UserType = "Teacher",
                    teacher.Name,
                    UserId = teacher.Id,
                    teacher.Email,
                    teacher.Subject
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error in registration: {ex.Message}");
                return BadRequest(new { Message = ex.Message });
            }
        }
       

        #endregion

        // Parent Register
        #region
        [HttpPost("register/parent")]
        public async Task<IActionResult> RegisterParent([FromBody] ParentRegisterDto request)
        {
            try
            {
                var parent = await _parentRegistrationService.RegisterAsync(request);

                // Use the Singleton JwtTokenGenerator
                var token = JwtTokenGenerator.GetInstance(_configuration).GenerateJwtToken(parent);

                await _notificationService.NotifyParentRegistered(parent.Email, parent.Name);

                return CreatedAtAction(nameof(Login), new
                {
                    Token = token,
                    UserType = "Parent",
                    UserId = parent.Id,
                    parent.studentId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        #endregion
    }
}
