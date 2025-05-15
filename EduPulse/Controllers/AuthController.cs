using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EduPulse.Models.DTOs;
using EduPulse.Models;
using EduPulse.Models.Service_Registration;
using EduPulse.Models.Services;
using Microsoft.AspNetCore.Authorization;

namespace EduPulse.Models.Users
{
    [ApiController]
    [Route("api/[controller]")]
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

        // Logout
        #region
        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            // In a token-based authentication system like JWT, 
            // the server doesn't maintain session state.
            // Clients should simply discard the token on their side.
            
            return Ok(new { message = "Logged out successfully" });
        }
        #endregion

        // Forgot Password
        #region
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto request)
        {
            if (string.IsNullOrEmpty(request.Email))
                return BadRequest(new { message = "Email is required" });

            var user = await _context.Students.FirstOrDefaultAsync(u => u.Email == request.Email) as IUser
                    ?? await _context.Teachers.FirstOrDefaultAsync(u => u.Email == request.Email) as IUser
                    ?? await _context.Parents.FirstOrDefaultAsync(u => u.Email == request.Email) as IUser;

            if (user == null)
                return NotFound(new { message = "User not found" });

            // Generate a reset token (in a real app, this would be a secure random token)
            var resetToken = Guid.NewGuid().ToString();
            
            // In a real implementation, store this token with an expiry time in the database
            // For now, we'll just simulate sending an email with a reset link
            
            try
            {
                await _notificationService.SendPasswordResetEmail(user.Email, user.Name, resetToken);
                return Ok(new { message = "Password reset instructions sent to your email" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to send reset email", error = ex.Message });
            }
        }
        #endregion

        // Change Password
        #region
        [HttpPut("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            IUser? user = null;
            
            // Find the user based on their role
            if (userRole == "Student")
                user = await _context.Students.FindAsync(userId);
            else if (userRole == "Teacher")
                user = await _context.Teachers.FindAsync(userId);
            else if (userRole == "Parent")
                user = await _context.Parents.FindAsync(userId);

            if (user == null)
                return NotFound(new { message = "User not found" });

            // Verify current password
            if (!VerifyPassword(request.CurrentPassword, user.Password))
                return BadRequest(new { message = "Current password is incorrect" });

            // Update password
            user.Password = HashPassword(request.NewPassword);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Password changed successfully" });
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
                Console.WriteLine($"Starting registration for student: {request.Name} ({request.Email})");
                var student = await _studentRegistrationService.RegisterAsync(request);
                Console.WriteLine($"Student registered successfully, ID: {student.Id}");

                var response = new
                {
                    UserType = "Student",
                    UserId = student.Id,
                    Name = student.Name,
                    Email = student.Email
                };

                try
                {
                    Console.WriteLine($"Sending welcome notification to student: {student.Name} ({student.Email})");
                    await _notificationService.NotifyStudentRegistered(student.Email, student.Name);
                   
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ ERROR in email notification: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");
                    // We continue despite email failure
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
        [HttpPost("teacher")]
        public async Task<IActionResult> RegisterTeacher([FromBody] TeacherRegisterDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var teacher = await _teacherRegistrationService.RegisterAsync(request);
                
                Console.WriteLine("✅ Preparing to notify teacher...");

                await _notificationService.NotifyTeacherRegistered(teacher.Email, teacher.Name, teacher.Subject);

                Console.WriteLine("✅ Email notification sent.");

                return CreatedAtAction(nameof(Login), new
                {
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
        [HttpPost("parent")]
        public async Task<IActionResult> RegisterParent([FromBody] ParentRegisterDto request)
        {
            try
            {
                var parent = await _parentRegistrationService.RegisterAsync(request);

                await _notificationService.NotifyParentRegistered(parent.Email, parent.Name);

                return CreatedAtAction(nameof(Login), new
                {
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
    
    public class ForgotPasswordDto
    {
        public string Email { get; set; } = string.Empty;
    }

    public class ChangePasswordDto
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
