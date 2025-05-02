using EduPulse.Models.Users;

namespace EduPulse.Models.Exam_Sub
{
    public class ExamResult
    {

        public int Id { get; set; }
        public int StudentId { get; set; }
        public int ExamId { get; set; }
        public int Grade { get; set; }
        public DateTime DateTaken { get; set; } 

        public virtual Student Student { get; set; }
        public virtual Exam Exam { get; set; }
    }
}
