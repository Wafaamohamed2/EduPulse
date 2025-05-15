using System;

namespace EduPulse.Models.DTOs
{
    public class ExamResponseDto
    {
        public int Id { get; set; }
        public string ExamName { get; set; } = string.Empty;
        public int TimeLimitInMinutes { get; set; }
        public string GoogleFormLink { get; set; } = string.Empty;
        public DateTime DeadlineDate { get; set; }
        public int TeacherId { get; set; }
        public int? SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public int Grade { get; set; }
    }
} 