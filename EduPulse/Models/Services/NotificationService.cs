using EduPulse.Models.Users;
using EduPulse.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace EduPulse.Models.Services
{
    public class NotificationService
    {
        private readonly IEmailService _emailService;
        private readonly FirebaseService? _firebaseService;
        private readonly SW_Entity? _context;

        public NotificationService(IEmailService emailService, FirebaseService? firebaseService = null, SW_Entity? context = null)
        {
            _emailService = emailService;
            _firebaseService = firebaseService;
            _context = context;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            await _emailService.SendEmailAsync(toEmail, subject, body);
        }

        public async Task SendPushNotificationAsync(int userId, string title, string body, Dictionary<string, string>? data = null)
        {
            if (_firebaseService == null || _context == null)
                return;

            try
            {
                var devices = await _context.UserDevices
                    .Where(d => d.UserId == userId && d.IsActive)
                    .ToListAsync();

                if (!devices.Any())
                    return;

                var tokens = devices.Select(d => d.DeviceToken).ToList();
                await _firebaseService.SendMulticastNotificationAsync(tokens, title, body, data);
            }
            catch (Exception)
            {
                // Log the error but don't fail the process
            }
        }

        public async Task NotifyTeacherRegistered(string email, string name, string subject)
        {
            string subjectLine = "Welcome to EduPulse 🎓";
            string body = $"<h3>Dear {name},</h3><p>Welcome to EduPulse! You've been successfully registered as a teacher for the {subject} subject. We are excited to have you on board! .. Let's inspire and educate together! 🎓</p>"  +
                $"<p>Best regards,<br>EduPulse Team</p>";

            await _emailService.SendEmailAsync(email, subjectLine, body);
        }

        public async Task NotifyStudentRegistered(string email, string name, int userId = 0)
        {
            try
            {
                Console.WriteLine($"Sending welcome email to student: {name} ({email})");
                
                string subjectLine = "Welcome to EduPulse 🎓";
                string body = $"<h3>Dear {name},</h3><p>Congratulations!" +
                    $" You've been successfully registered as a student in EduPulse. We are excited to have you with us and wish you a successful and inspiring learning journey! 📚</p>" +
                    $"<p>Best regards,<br>EduPulse Team</p>";
                
                await _emailService.SendEmailAsync(email, subjectLine, body);
                
                Console.WriteLine($"Welcome email sent successfully to {name} ({email})");

                if (userId > 0)
                {
                    await SendPushNotificationAsync(
                        userId, 
                        "Welcome to EduPulse 🎓", 
                        "You've been successfully registered as a student. We're excited to have you with us!"
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR sending welcome email to {name} ({email}): {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                // We don't rethrow to prevent registration failure due to email issues
            }
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

            // Send push notification to student
            if (student.Id > 0)
            {
                await SendPushNotificationAsync(
                    student.Id,
                    "Attendance Alert",
                    $"Your attendance rate is currently {absenceRate:F2}%. Please improve your attendance."
                );
            }
        }

        public async Task NotifyParentOfLowScore(string parentEmail, string parentName, string studentName, string subject, double score, int studentId = 0)
        {
            string subjectLine = "Academic Performance Alert";
            string body = $"<h3>Dear {parentName},</h3><p>Your child {studentName} has scored {score}% in the {subject} subject. Please consider discussing this with them to improve their performance.</p>" + $"<p>Best regards,<br>EduPulse Team</p>";
            await _emailService.SendEmailAsync(parentEmail, subjectLine, body);

            // Send push notification to student
            if (studentId > 0)
            {
                await SendPushNotificationAsync(
                    studentId,
                    "Academic Performance Alert",
                    $"You've scored {score}% in {subject}. Let's work on improving this!"
                );
            }
        }

        public async Task SendProfileUpdatedNotification(string email)
        {
            string subjectLine = "Profile Updated Successfully";
            string body = $"<h3>Dear User,</h3><p>Your profile has been updated successfully.</p>" + 
                         $"<p>Best regards,<br>EduPulse Team</p>";
            await _emailService.SendEmailAsync(email, subjectLine, body);
        }

        public async Task SendPasswordResetEmail(string email, string name, string resetToken)
        {
            string subjectLine = "Password Reset Request";
            string body = $"<h3>Dear {name},</h3><p>You requested a password reset. Please use the following token to reset your password:</p>" +
                         $"<p><strong>{resetToken}</strong></p>" +
                         $"<p>If you did not request this reset, please ignore this email.</p>" +
                         $"<p>Best regards,<br>EduPulse Team</p>";
            await _emailService.SendEmailAsync(email, subjectLine, body);
        }
    }
}
