using EduPulse.Models.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EduPulse.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace EduPulse.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : Controller
    {
        private readonly SW_Entity _context;
        private readonly NotificationService _notificationService;
        public StudentController(SW_Entity context , NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        // Record attendance for a student using their fingerId
        #region
        [HttpPost("record-attendance")]
        public IActionResult RecordAttendance([FromBody] AttendanceRequest request)
        {

            if (string.IsNullOrEmpty(request.FingerId))
                return BadRequest("FingerId is required");


            // check if student has fingerid record , if the fingerId is invalid 
            var student = _context.Students.AsNoTracking().FirstOrDefault(s => s.FingerId == request.FingerId);
            if (student == null)
            {
                return NotFound("Student not found.");
            }

            var attendance = new AttendenceRecord
            {

                TeacherId = request.TeacherId,
                AttendenceDate = DateTime.Now,
                IsPresent = request.IsPresent,
                FingerId = request.FingerId,
                StudentId = student.Id,

            };

            _context.AttendenceRecords.Add(attendance);
            _context.SaveChanges();

            return Ok(new { message = "Attendance recorded" });
        }


        public class AttendanceRequest
        {
            public string FingerId { get; set; } = string.Empty;
            public int TeacherId { get; set; }
            public bool IsPresent { get; set; }
        }

        #endregion


        // calculate absence rate for a student
        #region
        [HttpGet("absence-rate")]
        public async Task<IActionResult> GetAbsenceRate(string fingerId, DateTime startDate, DateTime endDate)
        {
            var student = _context.Students
                .Include(s => s.parent)  // change to include the parent
                .FirstOrDefault(s => s.FingerId == fingerId);

            if (student == null)
            {
                return NotFound("Student not found.");
            }

            // calculate the total number of days in the range
            var totalDays = (int)(endDate - startDate).TotalDays + 1;

            // calculate the number of days the student was present
            var presentDays = _context.AttendenceRecords.Count(r => r.StudentId == student.Id &&
                r.AttendenceDate.Date >= startDate.Date && r.AttendenceDate.Date <= endDate.Date && r.IsPresent);

            // calculate the number of days the student was absent
            int absentDays = totalDays - presentDays;
            double absenceRate = (absentDays / (double)totalDays) * 100;

            // check if the absence rate exceeds 25%
            if (absenceRate > 25)
            {
                await _notificationService.NotifyParentForAbsenceAsync(student, absenceRate);
            }

            return Ok(new { AbsenceRate = absenceRate });
        }


        #endregion


        [HttpGet("AvailableExams")]
        public async Task<IActionResult> GetAvailableExams()
        {
            
            var studentId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var student = await _context.Students
                .Include(s => s.Exams)
                .FirstOrDefaultAsync(s => s.Id == studentId);

            if (student == null)
                return NotFound("Student not found");

            var exams = await _context.Exams
                .Where(e => !e.IsExamExpired())
                .ToListAsync();

            return Ok(exams);
        }

        // New Endpoints Below

        [HttpPost("set-fingerprint")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> SetFingerprint([FromBody] FingerprintRequest request)
        {
            if (string.IsNullOrEmpty(request.FingerprintData))
                return BadRequest(new ApiResponse<object>(
                    success: false,
                    data: null,
                    message: "Fingerprint data is required",
                    statusCode: 400));

            var studentId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            var student = await _context.Students.FindAsync(studentId);
            if (student == null)
                return NotFound(new ApiResponse<object>(
                    success: false,
                    data: null,
                    message: "Student not found",
                    statusCode: 404));

            // Check if fingerprint ID already exists for another student
            var existingFingerprint = await _context.Students
                .AnyAsync(s => s.FingerId == request.FingerprintData && s.Id != studentId);
                
            if (existingFingerprint)
                return BadRequest(new ApiResponse<object>(
                    success: false,
                    data: null,
                    message: "This fingerprint is already registered to another student",
                    statusCode: 400));

            student.FingerId = request.FingerprintData;
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>(
                success: true,
                data: null,
                message: "Fingerprint set successfully",
                statusCode: 200));
        }

        public class FingerprintRequest
        {
            public string FingerprintData { get; set; } = string.Empty;
        }

      

   
    }
}
