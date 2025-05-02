using EduPulse.Models.Exam_Sub;
using SW_Project.Models;

namespace EduPulse.Models.Users
{
    public class Student : IUser
    {
        //Properties
        #region       
        public int Id { get; set; }
        public string? FingerId { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Confirm_Password { get; set; } = string.Empty;
        public int Grade { get; set; }
        private string? Subject { get; set; }

        public int Level { get; set; }
        public int Absences { get; set; }

        #endregion

        //Constructor
        #region
        public Student()
        {
        }
        public Student(int id, string name, string email, string password, int grade, string subject, int level, int absences)
        {
            Id = id;
            Name = name;
            Email = email;
            Password = password;
            Grade = grade;
            Subject = subject;
            Level = level;
            Absences = absences;
        }
        #endregion


        // Registration Methods
        #region
        public bool Login(string email, string password)
        {
            if (Email == email && Password == password)
            {
                Console.WriteLine(" login successful!");
                return true;
            }
            Console.WriteLine(" login failed!");
            return false;
        }

        public bool SignUp(string name, string email, string password)
        {
            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
            {
                Name = name;
                Email = email;
                Password = password;
                Console.WriteLine(" signup successful!");
                return true;
            }
            Console.WriteLine(" signup failed!");
            return false;
        }
        #endregion



        // Exam Methods
        #region
        public void Do_Quiz(Exam exam)
        {
            // Redirect the student to the Google Form link for the exam
            if (string.IsNullOrEmpty(exam.GoogleFormLink))
            {
                Console.WriteLine("No Google Form link available for this exam.");
                return;
            }

            // In real implementation, you would generate a notification to inform the student
            Console.WriteLine($"Student {Name} is doing the quiz: {exam.ExamName}. You have {exam.TimeLimitInMinutes} minutes.");
            Console.WriteLine($"Please visit the following link to take the quiz: {exam.GoogleFormLink}");
            Console.WriteLine("Once you've completed the quiz, you can view your results.");


        }
        public async Task<int> Preview_Quiz(Exam exam)
        {
            // Fetch the grade from Google Sheets (assuming it's stored after the quiz is completed)
            if (string.IsNullOrEmpty(exam.GoogleFormLink))
            {
                Console.WriteLine("No Google Form link available for this exam.");
                return -1; // Invalid grade
            }

            // Use the Google Sheets API to get the grade
            int grade = await GoogleSheetsService.GetGradeFromGoogleSheet("your-spreadsheet-id", this.Id);

            Console.WriteLine($"Student {Name} finished the quiz with grade: {grade} out of 100.");

            // Save the grade in the database
            await SaveExamResult(exam, grade);

            return grade;
        }


        private async Task SaveExamResult(Exam exam, int grade)
        {
            // Save grade to the ExamResults table
            Console.WriteLine($"Saving result for student {Name} in exam: {exam.ExamName} with grade: {grade}");

            var result = new ExamResult
            {
                StudentId = this.Id,
                ExamId = exam.Id,
                Grade = grade,
                DateTaken = DateTime.Now
            };
            //// Save the result to the database (Example code)
            //context.ExamResults.Add(result);
            //await context.SaveChangesAsync();
        }




        #endregion




        public void ViewStdInf()
        {
            Console.WriteLine($"Student Info: ID={Id}, Name={Name}, Level={Level}");
        }




        // Relationships
        #region
        // one to one relationship with parent

        public Parent? parent { get; set; } 

        // one to many relationship with attendence
        public ICollection<Attendence> Attendances { get; set; } = new List<Attendence>();

        // many to many relationship with exam
        public ICollection<Exam> Exams { get; set; } = new List<Exam>();

        public ICollection<StudentSubject> StudentSubjects { get; set; } = new List<StudentSubject>();


        #endregion
    }
}
