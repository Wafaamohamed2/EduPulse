namespace EduPulse.Models.Exam_Sub
{
    public interface IExamSystem
    {
        string CreateExam(string title, int duration);
        string GetExamLink(string examId);

    }
}
