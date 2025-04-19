namespace EduPulse.Models.Exam_Sub
{
    public class GoogleFormsExamAdapter : IExamSystem
    {
        private readonly string _googleFormUrl;

        public GoogleFormsExamAdapter(string googleFormUrl)
        {
            _googleFormUrl = googleFormUrl;
        }

        public string CreateExam(string title, int duration)
        {
            // Logic to create a Google Form exam
            // This is a placeholder implementation
            return $"Google Form created with title: {title} and duration: {duration} minutes. URL: {_googleFormUrl}";
        }

        public string GetExamLink(string examId)
        {
            // Logic to get the Google Form link for the exam
            // This is a placeholder implementation
            return $"Google Form link for exam ID {examId}: {_googleFormUrl}";
        }


    }
}
