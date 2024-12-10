using System;

namespace ForMiraiProject.ViewModels
{
    public class LoginResponseViewModel
    {
        private string _token = string.Empty;
        private string _refreshToken = string.Empty;
        private DateTime _expiry;

        /// <summary>
        /// The JWT token issued after successful login.
        /// </summary>
        public string Token
        {
            get => _token;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Token cannot be null or empty.", nameof(value));
                _token = value;
            }
        }

        /// <summary>
        /// The Refresh Token issued for extended sessions.
        /// </summary>
        public string RefreshToken
        {
            get => _refreshToken;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("RefreshToken cannot be null or empty.", nameof(value));
                _refreshToken = value;
            }
        }

        /// <summary>
        /// The expiration date and time of the issued JWT token.
        /// </summary>
        public DateTime Expiry
        {
            get => _expiry;
            set
            {
                if (value <= DateTime.UtcNow)
                    throw new ArgumentException("Expiry must be a future date and time.", nameof(value));
                _expiry = value;
            }
        }

        /// <summary>
        /// Default constructor for deserialization purposes.
        /// </summary>
        public LoginResponseViewModel() { }

        /// <summary>
        /// Constructor to initialize the LoginResponseViewModel.
        /// </summary>
        /// <param name="token">The JWT token.</param>
        /// <param name="refreshToken">The Refresh Token.</param>
        /// <param name="expiry">The token expiry date and time.</param>
        public LoginResponseViewModel(string token, string refreshToken, DateTime expiry)
        {
            Token = token ?? throw new ArgumentNullException(nameof(token), "Token cannot be null.");
            RefreshToken = refreshToken ?? throw new ArgumentNullException(nameof(refreshToken), "RefreshToken cannot be null.");
            Expiry = expiry > DateTime.UtcNow
                ? expiry
                : throw new ArgumentException("Expiry must be a future date and time.", nameof(expiry));
        }
    }
}
