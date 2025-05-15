using EduPulse.Models.DTOs;
using EduPulse.Models.Exam_Sub;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EduPulse.Models;
using System.Security.Claims;

namespace EduPulse.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ExamController : ControllerBase
    {
        private readonly SW_Entity _context;

        public ExamController(SW_Entity context)
        {
            _context = context;
        }

        // POST: api/Exam/Create
        [HttpPost("Create")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> CreateExam([FromBody] ExamDto dto)
        {
            var teacherId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var exam = new Exam
            {
                ExamName = dto.ExamName,
                TimeLimitInMinutes = dto.TimeLimitInMinutes,
                GoogleFormLink = dto.GoogleFormLink,
                DeadlineDate = dto.DeadlineDate,
                TeacherId = teacherId
            };

            _context.Exams.Add(exam);
            await _context.SaveChangesAsync();

            return Ok(exam);
        }



        // GET: api/Exam/Available/{studentId}
        [HttpGet("Available")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetAvailableExams()
        {

            var studentId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            
            var student = await _context.Students
                .Include(s => s.Exams)
                .FirstOrDefaultAsync(s => s.Id == studentId);

            if (student == null)
                return NotFound("Student not found");

            var availableExams = student.Exams
                .Where(e => !e.IsExamExpired())
                .ToList();

            return Ok(availableExams);
        }



        // POST: api/Exam/TakeExam
        [HttpPost("TakeExam")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> TakeExam(int examId)
        {
            // استخرج StudentId من الـ JWT
            var studentId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            // جلب الطالب مع الامتحانات المرتبطة
            var student = await _context.Students.Include(s => s.Exams).FirstOrDefaultAsync(s => s.Id == studentId);
            if (student == null)
                return NotFound("Student not found");

            // جلب الامتحان
            var exam = await _context.Exams.FirstOrDefaultAsync(e => e.Id == examId);
            if (exam == null)
                return NotFound("Exam not found");

            // التحقق إذا كان الامتحان منتهي
            if (exam.IsExamExpired())
                return BadRequest("This exam has expired.");

            // التأكد من أن الطالب لم يبدأ الامتحان بالفعل
            if (student.Exams.Any(e => e.Id == examId))
                return BadRequest("You have already started this exam.");

            // إضافة الامتحان للطالب
            student.Exams.Add(exam);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Exam started", link = exam.GoogleFormLink });
        }

       

    }

}


      