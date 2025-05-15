using EduPulse.Models.Users;

namespace EduPulse.Models
{
    public class AttendenceRecord
    {
        public int Id { get; set; }

        public DateTime AttendenceDate { get; set; }
        public bool IsPresent { get; set; }
        public string FingerId { get; set; } = string.Empty; 


        public int StudentId { get; set; }
        public Student Student { get; set; } = new Student();

        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; } = new Teacher();

    }
}
