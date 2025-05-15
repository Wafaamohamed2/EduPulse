using EduPulse.Models.Exam_Sub;
using EduPulse.Models;

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
        public string PhoneNumber { get; set; } = string.Empty;
        public string ProfilePictureUrl { get; set; } = string.Empty;
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
        public Exam Create_Quiz(string examName, int ExameTime, string googleFormLink, DateTime deadline)
        {
            var exam = new Exam
            {
                ExamName = examName,
                TimeLimitInMinutes = ExameTime,
                GoogleFormLink = googleFormLink ,
                DeadlineDate = deadline,
                TeacherId = this.Id
            };

            Console.WriteLine($"Quiz {examName} created successfully with link to Google Form: {googleFormLink} and deadline: {deadline}");
            return exam;
        }

        public void SaveExamToDB(SW_Entity context, Exam exam)
        {
            context.Exams.Add(exam);
            context.SaveChanges();
        }


        #endregion

        // Exams created by this teacher
        public ICollection<Exam> Exams { get; set; } = new List<Exam>();

        public ICollection<StudentSubject> StudentSubjects { get; set; } = new List<StudentSubject>();
    }
}
