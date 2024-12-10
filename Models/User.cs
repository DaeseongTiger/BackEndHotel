using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ForMiraiProject.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } // Primary Key

        [Required(ErrorMessage = "Username is required.")]
        [MaxLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
        public string Username { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public string FullName { get; set; } = string.Empty;

        

        private string _email = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [MaxLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        public string Email
        {
            get => _email;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Email cannot be empty.");

                if (!IsValidEmail(value))
                    throw new ArgumentException("Invalid email format.");

                _email = value.ToLower().Trim(); // Normalize to lower case
            }
        }

        [Required(ErrorMessage = "Password is required.")]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [MaxLength(15, ErrorMessage = "Phone number cannot exceed 15 characters.")]
        public string PhoneNumber { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        [MaxLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string Address { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(20)]
        public string Role { get; set; } = "User"; // Default Role

        public ICollection<Booking> Bookings { get; set; } = new HashSet<Booking>();
        public ICollection<Feedback> Feedbacks { get; set; } = new HashSet<Feedback>();

        // Deactivate user account
        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        // Update user profile
        public void UpdateProfile(string username, string phoneNumber, string address, DateTime dateOfBirth)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty.");

            if (!string.IsNullOrWhiteSpace(phoneNumber) && phoneNumber.Length > 15)
                throw new ArgumentException("Phone number is too long.");

            if (!string.IsNullOrWhiteSpace(address) && address.Length > 200)
                throw new ArgumentException("Address is too long.");

            if (dateOfBirth >= DateTime.UtcNow)
                throw new ArgumentException("Date of birth must be in the past.");

            Username = System.Net.WebUtility.HtmlEncode(username.Trim());
            PhoneNumber = phoneNumber?.Trim() ?? string.Empty;
            Address = System.Net.WebUtility.HtmlEncode(address.Trim());
            DateOfBirth = dateOfBirth;
            UpdatedAt = DateTime.UtcNow;
        }

        // Set or update password
        public void SetPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty.");

            if (!ValidatePasswordComplexity(password))
                throw new ArgumentException("Password must be at least 8 characters long and include an uppercase letter, a number, and a special character.");

            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            UpdatedAt = DateTime.UtcNow;
        }

        // Verify password
        public bool VerifyPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty.");

            return BCrypt.Net.BCrypt.Verify(password, PasswordHash);
        }

        // Private helper methods
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool ValidatePasswordComplexity(string password)
        {
            return password.Length >= 8 &&
                   password.Any(char.IsUpper) &&
                   password.Any(char.IsDigit) &&
                   password.Any(ch => !char.IsLetterOrDigit(ch));
        }
    }
}
