using EduPulse.Models.Users;
using System.ComponentModel.DataAnnotations;

namespace EduPulse.Models.Exam_Sub
{
    public class Exam
    {
        //Properties
        #region

        [Key]
        public int Id { get; set; }

        [Required]
        public string ExamName { get; set; }

        [Range(1, int.MaxValue)][Required]
        public int TimeLimitInMinutes { get; set; }

        [Required]
        [Url]
        public string GoogleFormLink { get; set; }

        public DateTime DeadlineDate { get; set; }

        // Foreign key to Teacher
        public int? TeacherId { get; set; }
        public Teacher? Creator { get; set; }
        #endregion



        public Exam() { }
        public Exam(string examname, int examtime , string googleFormLink , DateTime deadLine, int teacherId)
        {

            ExamName = examname;
            TimeLimitInMinutes = examtime;
            GoogleFormLink = googleFormLink;
            DeadlineDate = deadLine;
            TeacherId = teacherId;
        }

        // Method to check if the exam is expired based on the current date
        public bool IsExamExpired()
        {
            return DateTime.UtcNow > DeadlineDate.ToUniversalTime();
        }


        //many to many relationship with student
        public ICollection<Student> Students { get; set; } = new List<Student>();
    }
}
