using ForMiraiProject.Models;
using ForMiraiProject.Repositories.Interfaces;
using ForMiraiProject.Services.Interfaces;
using ForMiraiProject.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ForMiraiProject.Services
{
    public class AuthService : I_AuthService
    {
        private readonly I_AuthRepository _authRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(I_AuthRepository authRepository, IConfiguration configuration, ILogger<AuthService> logger)
        {
            _authRepository = authRepository ?? throw new ArgumentNullException(nameof(authRepository));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<RegisterResponseViewModel> RegisterAsync(RegisterViewModel model)
        {
            var newUser = new User
            {
                Email = model.Email.Trim().ToLower(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                FullName = model.FullName.Trim(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var success = await _authRepository.RegisterUserAsync(newUser);

            if (!success)
            {
                _logger.LogWarning("Failed to register user: {Email}", model.Email);
                throw new InvalidOperationException("Registration failed.");
            }

            _logger.LogInformation("User registered successfully: {Email}", model.Email);
            return new RegisterResponseViewModel(newUser.Id);
        }

        public async Task<LoginResponseViewModel?> LoginAsync(LoginViewModel model)
        {
            var user = await _authRepository.LoginUserAsync(model.Email.Trim().ToLower(), model.Password);

            if (user == null)
            {
                _logger.LogWarning("Invalid login attempt for email: {Email}", model.Email);
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            var token = GenerateJwtToken(user);
            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            var updated = await _authRepository.UpdateRefreshTokenAsync(user);

            if (!updated)
            {
                _logger.LogError("Failed to update refresh token for user: {Email}", user.Email);
                throw new InvalidOperationException("Login failed.");
            }

            _logger.LogInformation("User logged in successfully: {Email}", model.Email);
            return new LoginResponseViewModel(token, user.RefreshToken, DateTime.UtcNow.AddMinutes(GetTokenExpiry()));
        }

        public async Task<TokenResponseViewModel?> RefreshTokenAsync(RefreshTokenViewModel model)
        {
            var isValid = await _authRepository.ValidateRefreshTokenAsync(model.UserId, model.RefreshToken);

            if (!isValid)
            {
                _logger.LogWarning("Invalid or expired refresh token for user ID: {UserId}", model.UserId);
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");
            }

            var user = await _authRepository.GetUserByIdAsync(model.UserId);

            if (user == null)
            {
                _logger.LogWarning("Failed to retrieve user for refresh token.");
                throw new UnauthorizedAccessException("Invalid refresh token.");
            }

            var newToken = GenerateJwtToken(user);
            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            var refreshTokenUpdated = await _authRepository.UpdateRefreshTokenAsync(user);

            if (!refreshTokenUpdated)
            {
                _logger.LogError("Failed to update refresh token for user ID: {UserId}", model.UserId);
                throw new InvalidOperationException("Failed to refresh token.");
            }

            _logger.LogInformation("Token refreshed successfully for user ID: {UserId}", model.UserId);
            return new TokenResponseViewModel(newToken, user.RefreshToken, GetTokenExpiry());
        }

        public void Logout(ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Logout failed: Invalid user identifier.");
                throw new UnauthorizedAccessException("Invalid user identifier.");
            }

            var logoutSuccessful = _authRepository.LogoutUserAsync(new User { Id = Guid.Parse(userId) }).Result;

            if (!logoutSuccessful)
            {
                _logger.LogError("Failed to logout user ID: {UserId}", userId);
                throw new InvalidOperationException("Logout failed.");
            }

            _logger.LogInformation("User logged out successfully: {UserId}", userId);
        }

        private string GenerateJwtToken(User user)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key is missing."));
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Name, user.FullName)
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(GetTokenExpiry()),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate JWT token.");
                throw new InvalidOperationException("Failed to generate JWT token.", ex);
            }
        }

        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[32];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes);
        }

        private int GetTokenExpiry()
        {
            return int.TryParse(_configuration["Jwt:TokenExpiryInMinutes"], out var expiry) ? expiry : 15; // Default 15 minutes
        }
    }
}
