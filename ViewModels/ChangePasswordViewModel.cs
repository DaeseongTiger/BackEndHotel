using System;
using System.ComponentModel.DataAnnotations;

namespace ForMiraiProject.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Old password is required.")]
        [MinLength(8, ErrorMessage = "Old password must be at least 8 characters.")]
        public string OldPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required.")]
        [MinLength(8, ErrorMessage = "New password must be at least 8 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        [NotSameAsOldPassword("OldPassword", ErrorMessage = "New password cannot be the same as the old password.")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm password is required.")]
        [Compare("NewPassword", ErrorMessage = "New password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class NotSameAsOldPasswordAttribute : ValidationAttribute
    {
        public string OldPasswordPropertyName { get; }

        public NotSameAsOldPasswordAttribute(string oldPasswordPropertyName)
        {
            OldPasswordPropertyName = oldPasswordPropertyName;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // ตรวจสอบว่า value หรือ validationContext ไม่ใช่ null
            if (value == null)
            {
                return new ValidationResult("New password is required.");
            }

            var newPassword = value as string;
            var oldPasswordProperty = validationContext.ObjectType.GetProperty(OldPasswordPropertyName);

            if (oldPasswordProperty == null)
            {
                return new ValidationResult($"Property '{OldPasswordPropertyName}' does not exist.");
            }

            var oldPassword = oldPasswordProperty.GetValue(validationContext.ObjectInstance) as string;

            if (!string.IsNullOrEmpty(oldPassword) && oldPassword.Equals(newPassword))
            {
                return new ValidationResult("New password must not be the same as the old password.");
            }

            return ValidationResult.Success;
        }
    }
}
