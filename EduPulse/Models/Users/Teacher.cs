using EduPulse.Models.Exam_Sub;
using SW_Project.Models;

namespace EduPulse.Models.Users
{
    public class Teacher : IUser
    {
        // Properties
        #region
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Confirm_Password { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        #endregion


        // Registration Methods
        #region
        public bool Login(string email, string password)
        {

            if (Email == email && Password == password)
            {
                Console.WriteLine("login successful!");
                return true;
            }
            Console.WriteLine("login failed!");
            return false;
        }


        public bool SignUp(string name, string email, string password)
        {
            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                Name = name;
                Email = email;
                Password = password;
                Console.WriteLine("signup successful!");
                return true;
            }
            Console.WriteLine("signup failed!");
            return false;
        }
        #endregion


        // Teacher Methods
        #region
        public Exam Create_Quiz(string examName, int ExameTime, List<(string Question, string Answer)> questionsWithAnswers)
        {
            var exam = new Exam(examName, ExameTime);
            foreach (var question in questionsWithAnswers)
            {
                exam.AddQuestion(question.Question, question.Answer);
            }

            Console.WriteLine($"Quiz {examName} created successfully with {exam.QuestionsCount} questions ");
            return exam;
        }

        public void SaveExamToDB(SW_Entity context, Exam exam)
        {
            context.Exams.Add(exam);
            context.SaveChanges();

        }


        #endregion

        public ICollection<StudentSubject> StudentSubjects { get; set; } = new List<StudentSubject>();
    }
}
