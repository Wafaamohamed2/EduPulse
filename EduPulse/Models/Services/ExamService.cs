using EduPulse.Models.Exam_Sub;
using Microsoft.EntityFrameworkCore;

namespace EduPulse.Models.Services
{
    public class ExamService
    {
        private readonly SW_Entity _context;

        public ExamService(SW_Entity context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Exam>> GetAvailableExamsAsync(int studentId)
        {
            // Get all exams for teachers that the student is associated with
            var teacherIds = await _context.StudentSubjects
                .Where(ss => ss.StudentId == studentId)
                .Select(ss => ss.TeacherId)
                .ToListAsync();

            var exams = await _context.Exams
                .Where(e => teacherIds.Contains(e.TeacherId.Value) && !e.IsExamExpired())
                .ToListAsync();

            return exams;
        }
    }
} 