using System;
using System.ComponentModel.DataAnnotations;

namespace ForMiraiProject.ViewModels
{
    public class RefreshTokenViewModel
    {
        [Required(ErrorMessage = "Refresh token is required.")]
        [MaxLength(2048, ErrorMessage = "Refresh token cannot exceed 2048 characters.")]
        public string RefreshToken { get; set; } = string.Empty;

        [Required(ErrorMessage = "User ID is required.")]
        public Guid UserId { get; set; }

        // Validate format of the refresh token
        public void ValidateTokenFormat()
        {
            if (string.IsNullOrWhiteSpace(RefreshToken))
                throw new ArgumentException("Refresh token cannot be empty or null.");

            if (RefreshToken.Length > 2048)
                throw new ArgumentException("Refresh token length exceeds the maximum limit.");
        }

        // Validate the user ID
        public void ValidateUserId()
        {
            if (UserId == Guid.Empty)
                throw new ArgumentException("Invalid user ID.");
        }

        // Helper method to validate the entire ViewModel
        public void Validate()
        {
            ValidateTokenFormat();
            ValidateUserId();
        }

        // Default constructor
        public RefreshTokenViewModel() { }

        // Constructor with parameters for convenience
        public RefreshTokenViewModel(string refreshToken, Guid userId)
        {
            RefreshToken = refreshToken;
            UserId = userId;

            Validate(); // Ensure validation during object creation
        }
    }
}
