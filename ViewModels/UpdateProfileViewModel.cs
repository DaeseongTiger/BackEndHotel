using System;
using System.ComponentModel.DataAnnotations;

namespace ForMiraiProject.ViewModels
{
    public class UpdateProfileViewModel
    {
        [Required(ErrorMessage = "User ID is required.")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Full name is required.")]
        [MaxLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Full name can only contain letters and spaces.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [MaxLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "Invalid phone number format.")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of birth is required.")]
        [DataType(DataType.Date)]
        [CustomDateRange(ErrorMessage = "Date of birth cannot be in the future.")]
        public DateTime DateOfBirth { get; set; }

        [MaxLength(250, ErrorMessage = "Address cannot exceed 250 characters.")]
        public string Address { get; set; } = string.Empty;

        public string ProfilePictureUrl { get; set; } = string.Empty;

        [MaxLength(50, ErrorMessage = "Gender cannot exceed 50 characters.")]
        [RegularExpression(@"^(Male|Female|Other|Prefer not to say)$", ErrorMessage = "Invalid gender value.")]
        public string Gender { get; set; } = "Prefer not to say";

        public bool IsEmailVerified { get; set; } = false;
        public bool IsPhoneNumberVerified { get; set; } = false;
    }
}
