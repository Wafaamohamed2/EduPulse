using EduPulse.Models.Users;

namespace EduPulse.Models.Services
{
    public class NotificationService
    {
        private readonly IEmailService _emailService;

        public NotificationService(IEmailService emailService)
        {
            _emailService = emailService;
        }
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            await _emailService.SendEmailAsync(toEmail, subject, body);
        }
        public async Task NotifyTeacherRegistered(string email, string name, string subject)
        {
            string subjectLine = "Welcome to EduPulse 🎓";
            string body = $"<h3>Dear {name},</h3><p>Welcome to EduPulse! You've been successfully registered as a teacher for the {subject} subject. We are excited to have you on board! .. Let’s inspire and educate together! 🎓</p>"  +
                $"<p>Best regards,<br>EduPulse Team</p>";

            await _emailService.SendEmailAsync(email, subjectLine, body);
        }

        public async Task NotifyStudentRegistered(string email, string name)
        {
            string subjectLine = "Welcome to EduPulse 🎓";
            string body = $"<h3>Dear {name},</h3><p>Congratulations!" +
                $" You've been successfully registered as a student in EduPulse. We are excited to have you with us and wish you a successful and inspiring learning journey! 📚</p>" +
                $"<p>Best regards,<br>EduPulse Team</p>";
            await _emailService.SendEmailAsync(email, subjectLine, body);
        }

        public async Task NotifyParentRegistered(string email, string name)
        {
            string subjectLine = "Welcome to EduPulse 🎓";
            string body = $"<h3>Dear {name},</h3><p>Your child has been successfully registered in EduPulse. We look forward to working together for their success.</p>" + $"<p>Best regards,<br>EduPulse Team</p>";
            await _emailService.SendEmailAsync(email, subjectLine, body);
        }

        public async Task NotifyParentForAbsenceAsync(Student student, double absenceRate)
        {
            if (student?.parent?.Email == null)
                return;

            var subject = $"Absence Alert for {student.Name}";
            var body = $@"
            <h3>Dear {student.parent.Name},</h3>
            <p>This is to inform you that your child <strong>{student.Name}</strong> has exceeded the acceptable absence rate.</p>
            <p>Current Absence Rate: <strong>{absenceRate:F2}%</strong></p>
            <p>Please take the necessary actions to address this.</p>
            <br/>
            <p>Best regards,<br/>EduPulse Team</p>";

            await _emailService.SendEmailAsync(student.parent.Email, subject, body);
        }

        public async Task NotifyParentOfLowScore(string parentEmail, string parentName, string studentName, string subject, double score)
        {
            string subjectLine = "Academic Performance Alert";
            string body = $"<h3>Dear {parentName},</h3><p>Your child {studentName} has scored {score}% in the {subject} subject. Please consider discussing this with them to improve their performance.</p>" + $"<p>Best regards,<br>EduPulse Team</p>";
            await _emailService.SendEmailAsync(parentEmail, subjectLine, body);
        }
    }

}
