using EduPulse.Models.Users;

namespace EduPulse.Models.Exam_Sub
{
    public class StudentSubject
    {
        public int StudentId { get; set; }
        public Student Student { get; set; } = new Student();

        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; } = new Teacher();

    }
}
