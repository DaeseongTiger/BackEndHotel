using System.ComponentModel.DataAnnotations;

namespace ForMiraiProject.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [MaxLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
        [MaxLength(100, ErrorMessage = "Password cannot exceed 100 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        public string Password { get; set; } = string.Empty;

        // เพิ่มเพื่อความปลอดภัยในอนาคต เช่น MFA หรือการตรวจสอบอื่น ๆ
        [Required(ErrorMessage = "Login type is required.")]
        [RegularExpression(@"^(Standard|Google|Facebook|Microsoft)$", ErrorMessage = "Invalid login type.")]
        public string LoginType { get; set; } = "Standard";
    }
}
