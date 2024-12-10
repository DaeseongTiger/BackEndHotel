using System;
using System.ComponentModel.DataAnnotations;

namespace ForMiraiProject.ViewModels
{
    public class TokenResponseViewModel
    {
        [Required(ErrorMessage = "Access token is required.")]
        [MaxLength(2048, ErrorMessage = "Access token cannot exceed 2048 characters.")]
        public string AccessToken { get; set; } = string.Empty;

        [Required(ErrorMessage = "Refresh token is required.")]
        [MaxLength(2048, ErrorMessage = "Refresh token cannot exceed 2048 characters.")]
        public string RefreshToken { get; set; } = string.Empty;

        [Required(ErrorMessage = "Token type is required.")]
        [RegularExpression(@"^Bearer$", ErrorMessage = "Token type must be 'Bearer'.")]
        public string TokenType { get; set; } = "Bearer";

        [Required(ErrorMessage = "Token expiration time is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Expires in must be a positive integer.")]
        public int ExpiresIn { get; set; }

        [Required(ErrorMessage = "Issued at is required.")]
        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "Expiration time is required.")]
        public DateTime ExpirationTime { get; set; }

        /// <summary>
        /// Helper method to calculate expiration time from ExpiresIn.
        /// </summary>
        public void SetExpirationTime()
        {
            if (ExpiresIn <= 0)
                throw new InvalidOperationException("ExpiresIn must be greater than zero.");
            
            ExpirationTime = IssuedAt.AddSeconds(ExpiresIn);
        }

        /// <summary>
        /// Method to validate the format of tokens and token type.
        /// </summary>
        public void ValidateTokenFormat()
        {
            if (string.IsNullOrWhiteSpace(AccessToken))
                throw new ArgumentException("Access token cannot be empty or null.");

            if (string.IsNullOrWhiteSpace(RefreshToken))
                throw new ArgumentException("Refresh token cannot be empty or null.");

            if (!TokenType.Equals("Bearer", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Invalid token type. Only 'Bearer' is supported.");
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TokenResponseViewModel() { }

        /// <summary>
        /// Constructor with parameters for convenience.
        /// </summary>
        /// <param name="accessToken">The access token string.</param>
        /// <param name="refreshToken">The refresh token string.</param>
        /// <param name="expiresIn">The time in seconds until the token expires.</param>
        public TokenResponseViewModel(string accessToken, string refreshToken, int expiresIn)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            ExpiresIn = expiresIn;
            IssuedAt = DateTime.UtcNow;
            SetExpirationTime();
        }
    }
}
