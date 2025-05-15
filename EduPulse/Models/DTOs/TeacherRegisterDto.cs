using System.ComponentModel.DataAnnotations;
namespace EduPulse.Models.DTOs
{
    public class TeacherRegisterDto
    {

        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, ErrorMessage = "Name cannot be longer than 50 characters")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }


        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
        public string? Password { get; set; }


     

        [Required(ErrorMessage = "Subject is required")]
        public string? Subject { get; set; }
    }
}
