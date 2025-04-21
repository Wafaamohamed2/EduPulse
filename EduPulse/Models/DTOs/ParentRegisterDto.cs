using System.ComponentModel.DataAnnotations;

namespace EduPulse.Models.DTOs
{
    public class ParentRegisterDto
    {
        [Required]
        public string? Name { get; set; }

        [Required , EmailAddress]
        public string? Email { get; set; }

        [Required ,MinLength(6)]
          public string? Password { get; set; }
        [Required , Compare("Password")]
        public string? ConfirmPassword { get; set; }

        [Required]
        public int StudentId { get; set; }

    }
}
