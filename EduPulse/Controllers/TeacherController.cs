using EduPulse.Models.DTOs;
using EduPulse.Models.Exam_Sub;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SW_Project.Models;
using System.Security.Claims;

namespace EduPulse.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class TeacherController : ControllerBase
    {

        private readonly SW_Entity _context;

        public TeacherController(SW_Entity context)
        {
            _context = context;
        }



        [HttpPost("Create")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> CreateExam(ExamDto dto)
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

            return CreatedAtAction(nameof(CreateExam), new { id = exam.Id }, exam);

        }

    }
}
