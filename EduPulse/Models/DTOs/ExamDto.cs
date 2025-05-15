namespace EduPulse.Models.DTOs
{
    public class ExamDto
    {

       
        public string ExamName { get; set; }
        public int TimeLimitInMinutes { get; set; }
        public string GoogleFormLink { get; set; }
        public DateTime DeadlineDate { get; set; }
    }
}
