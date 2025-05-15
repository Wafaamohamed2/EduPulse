using System;

namespace EduPulse.Models.DTOs
{
    public class ExamNotificationDto
    {
        public string ExamName { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string GoogleFormLink { get; set; } = string.Empty;
        public DateTime ExamDate { get; set; }
        public int TimeLimitInMinutes { get; set; }
        public DateTime DeadlineDate { get; set; }
    }
} 