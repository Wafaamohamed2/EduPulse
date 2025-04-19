using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EduPulse.Models;
using EduPulse.Models.DTOs;  
using SW_Project.Models;
using Org.BouncyCastle.Crypto.Generators;
using EduPulse.Models.Users;
using StudentAttendanceAPI.Services;
using EduPulse.Models.Service_Registration;

namespace EduPulse.Models.Users
{
    [ApiController]
    [Route("register")]
    public class AuthController : Controller
    {
        private readonly SW_Entity _context;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;

        // Registration Services "used to separate se"
        private readonly StudentRegistrationService _studentRegistrationService;
        private readonly TeacherRegistrationService _teacherRegistrationService;
        private readonly ParentRegistrationService _parentRegistrationService;

        public AuthController(SW_Entity context, IConfiguration configuration , EmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
            
            _studentRegistrationService = new StudentRegistrationService(_context);
            _teacherRegistrationService = new TeacherRegistrationService(context);
            _parentRegistrationService = new ParentRegistrationService(context);

        }


        // Verify Password
        private bool VerifyPassword(string inputPassword, string storedPassword)
        {
           
             return BCrypt.Net.BCrypt.Verify(inputPassword, storedPassword);
        }

        // Hash Password
        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }



        // Generate JWT Token
        private string GenerateJwtToken(IUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.GetType().Name)
        };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }



        // This method sends a registration email to the user after successful registration.
        #region
        private async Task SendRegistrationEmailAsync(string recipientEmail, string recipientName)
        {
            var subject ="Wellcome in EduPulse!";
            var body = $"Hello {recipientName}،<br> Thanks for your registration with us .. wish you have a good experience.";

            await _emailService.SendEmailAsync(recipientEmail, subject, body);
        }
        #endregion



        // login
        #region
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _context.Students.FirstOrDefaultAsync(u => u.Email == loginDto.Email) as IUser
                    ?? await _context.Teachers.FirstOrDefaultAsync(u => u.Email == loginDto.Email) as IUser
                    ?? await _context.Parents.FirstOrDefaultAsync(u => u.Email == loginDto.Email) as IUser;

            if (user == null || !VerifyPassword(loginDto.Password, user.Password) || string.IsNullOrWhiteSpace(loginDto.Password))
                return Unauthorized("Invalid Information");

            var token = GenerateJwtToken(user);
            return Ok(new { Token = token, UserType = user.GetType().Name });
        }
        #endregion


        // student register
        #region
        [HttpPost("student")]
        public async Task<IActionResult> RegisterStudent([FromBody] StudentRegisterDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var student = await _studentRegistrationService.RegisterAsync(request);

                // إفصل الريسبونس عن EF Core Entity
                var response = new
                {
                    Token = GenerateJwtToken(student),
                    UserType = "Student",
                    UserId = student.Id,
                    Name = student.Name,
                    Email = student.Email
                };

                try
                {
                    await SendRegistrationEmailAsync(student.Email, student.Name);
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


        // Register Teacher
        #region
        [HttpPost("register/teacher")]
        public async Task<IActionResult> RegisterTeacher([FromBody] TeacherRegisterDto request)
        {
            

            // Validate the request model
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {

                var teacher = await _teacherRegistrationService.RegisterAsync(request);


                // Generate JWT token
                var token = GenerateJwtToken(teacher);


                // Send registration email successfully
                await SendRegistrationEmailAsync(teacher.Email, teacher.Name);


              
                // Return success response with token
                return CreatedAtAction(nameof(Login), new
                {
                    Token = token,
                    UserType = "Teacher",
                    UserId = teacher.Id,
                    teacher.Name,
                    teacher.Email,
                    teacher.Subject
                });
            }
            catch (Exception ex)
            {
                // Log the error
                return BadRequest(new { Message = ex.Message });
            }
        }
        #endregion



        // Register Parent
        #region
        [HttpPost("register/parent")]
        public async Task<IActionResult> RegisterParent([FromBody] ParentRegisterDto request)
        {
            try { 
                var parent = await _parentRegistrationService.RegisterAsync(request);

                // Send registration email successfully
                await SendRegistrationEmailAsync(parent.Email, parent.Name);


                 var token = GenerateJwtToken(parent);

                return CreatedAtAction(nameof(Login), new
                {
                    Token = token,
                    UserType = "Parent",
                    UserId = parent.Id,
                    StudentId = parent.studentId
               });
            
            }
            catch (Exception ex)
            {
                // Log the error
                return BadRequest(new { Message = ex.Message });
            }

          
        }
        #endregion
    }
}