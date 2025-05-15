namespace EduPulse.Models.Exam_Sub
{
    public class Subject
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

       
        public ICollection<StudentSubject> StudentSubjects { get; set; } = new List<StudentSubject>();

       
        public ICollection<Exam> Exams { get; set; } = new List<Exam>();
    }
}

