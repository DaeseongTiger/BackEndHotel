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
using ForMiraiProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using ForMiraiProject.Repositories;

namespace ForMiraiProject.Services
{
    public class AuthService : I_AuthService
    {
        private readonly I_AuthRepository _authRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(I_AuthRepository authRepository, IConfiguration configuration, ILogger<AuthService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _authRepository = authRepository ?? throw new ArgumentNullException(nameof(authRepository));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public bool HasRole(ClaimsPrincipal user, string role)
        {
            if (user == null || string.IsNullOrEmpty(role))
            {
                return false;
            }
            return user.IsInRole(role);
        }

        // Register new user
        public async Task<RegisterResponseViewModel> RegisterAsync(RegisterViewModel model)
        {
            var user = new User
            {
                Email = model.Email.Trim().ToLower(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                FullName = model.FullName.Trim(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var success = await _authRepository.RegisterUserAsync(user);

            if (!success)
            {
                _logger.LogWarning("Failed to register user: {Email}", model.Email);
                throw new InvalidOperationException("Registration failed.");
            }

            _logger.LogInformation("User registered successfully: {Email}", model.Email);
            return new RegisterResponseViewModel(user.Id);
        }

        // Login user and generate JWT token
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

        // Refresh token using refresh token
        public async Task<string> RefreshTokenAsync(RefreshTokenViewModel model)
        {
            // เพิ่ม interface method ใน I_AuthRepository
            var user = await _authRepository.GetUserByRefreshTokenAsync(model.RefreshToken);

            if (user == null)
            {
                _logger.LogWarning("Invalid refresh token: {RefreshToken}", model.RefreshToken);
                throw new UnauthorizedAccessException("Invalid refresh token.");
            }

            var newToken = GenerateJwtToken(user);
            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            var updated = await _authRepository.UpdateRefreshTokenAsync(user);

            if (!updated)
            {
                _logger.LogError("Failed to refresh token for user: {Email}", user.Email);
                throw new InvalidOperationException("Failed to refresh token.");
            }

            return newToken;
        }

        // Revoke the refresh token
        public async Task<bool> RevokeRefreshTokenAsync(string refreshToken)
        {
            var user = await _authRepository.GetUserByRefreshTokenAsync(refreshToken);

            if (user == null)
            {
                _logger.LogWarning("Invalid refresh token: {RefreshToken}", refreshToken);
                throw new UnauthorizedAccessException("Invalid refresh token.");
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiry = null;

            var updated = await _authRepository.UpdateRefreshTokenAsync(user);

            if (!updated)
            {
                _logger.LogError("Failed to revoke refresh token for user: {Email}", user.Email);
                throw new InvalidOperationException("Failed to revoke refresh token.");
            }

            return true;
        }

        // Validate JWT Token
        public Task<bool> ValidateJwtTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key is missing."));
                var validationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                return Task.FromResult(validatedToken is JwtSecurityToken jwtToken && jwtToken.ValidTo > DateTime.UtcNow);
            }
            catch (Exception ex)
            {
                _logger.LogError("Token validation failed: {Message}", ex.Message);
                return Task.FromResult(false);
            }
        }

        // Logout user
        public async Task Logout(ClaimsPrincipal principal)
        {
            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                _logger.LogWarning("Logout failed: User ID not found.");
                throw new InvalidOperationException("User ID not found.");
            }

            var user = await _authRepository.GetUserByIdAsync(Guid.Parse(userId));
            if (user == null)
            {
                _logger.LogWarning("Logout failed: User not found.");
                throw new InvalidOperationException("User not found.");
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiry = null;
            await _authRepository.UpdateRefreshTokenAsync(user);

            _logger.LogInformation("User logged out successfully: {Email}", user.Email);
        }

        // Validate CSRF token
        public Task<bool> ValidateCsrfTokenAsync(string token)
        {
            var csrfTokenFromSession = _httpContextAccessor.HttpContext?.Session.GetString("CSRF_TOKEN");
            return Task.FromResult(csrfTokenFromSession == token);
        }

        // Generate CSRF token
        public Task<string> GenerateCsrfTokenAsync()
        {
            try
            {
                var csrfToken = Guid.NewGuid().ToString();
                _httpContextAccessor.HttpContext?.Session.SetString("CSRF_TOKEN", csrfToken);
                _logger.LogInformation("Generated CSRF Token.");
                return Task.FromResult(csrfToken);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error generating CSRF token: {ex.Message}");
                throw new ApplicationException("Failed to generate CSRF token.", ex);
            }
        }

        // Generate JWT token
        private string GenerateJwtToken(User user)
        {
            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? string.Empty));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(GetTokenExpiry()),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Generate refresh token
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }
            return Convert.ToBase64String(randomNumber);
        }

        // Get token expiry time
        private int GetTokenExpiry()
        {
            return int.TryParse(_configuration["Jwt:ExpiryInMinutes"], out var expiry) ? expiry : 60;
        }

        // Get user by email
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                _logger.LogWarning("The provided email is null or empty.");
                return null;
            }

            var user = await _authRepository.GetUserByEmailAsync(email.Trim().ToLower());

            if (user == null)
            {
                _logger.LogWarning("No user found with the provided email: {Email}", email);
                return null;
            }

            _logger.LogInformation("Successfully retrieved user by email: {Email}", email);
            return user;
        }


        public async Task<User?> GetUserByIdAsync(Guid userId)
{
    if (userId == Guid.Empty)
    {
        _logger.LogWarning("The provided user ID is empty.");
        return null;
    }

    var user = await _authRepository.GetUserByIdAsync(userId);

    if (user == null)
    {
        _logger.LogWarning("No user found with the provided ID: {UserId}", userId);
        return null;
    }

    _logger.LogInformation("Successfully retrieved user by ID: {UserId}", userId);
    return user;
}


 public async Task<User?> GetUserByRefreshTokenAsync(string refreshToken)
    {
        if (string.IsNullOrEmpty(refreshToken))
        {
            _logger.LogWarning("The provided refresh token is null or empty.");
            return null;
        }

        var user = await _authRepository.GetUserByRefreshTokenAsync(refreshToken);
        
        if (user == null)
        {
            _logger.LogWarning("No user found with the provided refresh token.");
            return null;
        }

        if (user.RefreshTokenExpiry < DateTime.UtcNow)
        {
            _logger.LogWarning("The refresh token has expired for user: {Email}", user.Email);
            return null;
        }

        return user;
    }

    
    }
}