using System.ComponentModel.DataAnnotations;

namespace EduPulse.Models.DTOs
{
    public class UpdateUserProfileDto
    {

        [StringLength(100, MinimumLength = 2)]
        public string? Name { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        [Url]
        public string? ProfilePictureUrl { get; set; }

    }
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Current password is required")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$",
        ErrorMessage = "Password must contain uppercase, lowercase and number")]

        public string NewPassword { get; set; } = string.Empty;


        [Compare("NewPassword", ErrorMessage = "Passwords do not match")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }

    public class UpdateDeviceDto
    {
        [Required]
        public string DeviceToken { get; set; } = string.Empty;
    }
}
