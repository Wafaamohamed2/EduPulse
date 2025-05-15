using EduPulse.Models.Users;

namespace EduPulse.Models.Exam_Sub
{
    public class ExamStudent
    {

        public int StudentsId { get; set; }
        public Student Student { get; set; } = new Student();

        public int ExamsId { get; set; }
        public Exam Exam { get; set; } = new Exam();
    }
}
