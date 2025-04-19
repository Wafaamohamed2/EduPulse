using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SW_Project.Models;

namespace SW_Project.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : Controller
    {
        private readonly SW_Entity _context;
        public StudentController(SW_Entity context)
        {
            _context = context;
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
                FingerId = request.FingerId
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
        public IActionResult GetAbsenceRate(string fingerId, DateTime startDate, DateTime endDate)
        {
            var student = _context.Students.FirstOrDefault(s => s.FingerId == fingerId);
            if (student == null)
            {
                return NotFound("Student not found.");
            }

            var totalClasses = _context.Attendances
                .Count(a => a.Id == student.Id && a.Date >= startDate && a.Date <= endDate);

            var absentClasses = _context.Attendances
                .Count(a => a.Id == student.Id && a.Date >= startDate && a.Date <= endDate && !a.IsPresent);

            double absenceRate = totalClasses == 0 ? 0 : (absentClasses / (double)totalClasses) * 100;

            return Ok(new { AbsenceRate = absenceRate });
        }

        #endregion




       

    }
}
