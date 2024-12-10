using System;
using System.ComponentModel.DataAnnotations;

namespace ForMiraiProject.ViewModels
{
    public class UserProfileViewModel
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
        [CustomDateRange(ErrorMessage = "Date of birth must be in the past.")]
        public DateTime DateOfBirth { get; set; }

        [MaxLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string? Address { get; set; } = null;

        [Required(ErrorMessage = "Registration date is required.")]
        public DateTime RegistrationDate { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? LastLoginDate { get; set; }

        public bool IsActive { get; set; } = true;

        public string? ProfilePictureUrl { get; set; }
    }

    // Custom validation attribute for date rang
}
