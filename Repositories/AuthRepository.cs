using ForMiraiProject.Models;
using ForMiraiProject.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ForMiraiProject.Data;

namespace ForMiraiProject.Repositories
{
    public class AuthRepository : I_AuthRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AuthRepository> _logger;

        // คอนสตรัคเตอร์สำหรับรับ _context และ _logger
        public AuthRepository(AppDbContext context, ILogger<AuthRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Registers a new user.
        /// </summary>
        public async Task<bool> RegisterUserAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), "User cannot be null.");

            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
            {
                _logger.LogWarning("Attempted to register a user with an existing email: {Email}", user.Email);
                throw new InvalidOperationException("Email already exists.");
            }

            try
            {
                _context.Users.Add(user);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error registering user with email {Email}", user.Email);
                throw new InvalidOperationException("An error occurred while registering the user.", ex);
            }
        }

        /// <summary>
        /// Logs in a user by validating email and password.
        /// </summary>
        public async Task<User?> LoginUserAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty.", nameof(email));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty.", nameof(password));

            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email.Trim().ToLower());
                if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                {
                    _logger.LogWarning("Invalid login attempt for email: {Email}", email);
                    return null;
                }

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging in user with email {Email}", email);
                throw;
            }
        }

        /// <summary>
        /// Updates a user's refresh token and expiry date.
        /// </summary>
        public async Task<bool> UpdateRefreshTokenAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), "User cannot be null.");

            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            try
            {
                _context.Users.Update(user);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error updating refresh token for user {Email}", user.Email);
                throw new InvalidOperationException("An error occurred while updating the refresh token.", ex);
            }
        }

        /// <summary>
        /// Validates a user's refresh token.
        /// </summary>
        public async Task<bool> ValidateRefreshTokenAsync(Guid userId, string refreshToken)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));

            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new ArgumentException("Refresh token cannot be empty.", nameof(refreshToken));

            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId && u.RefreshToken == refreshToken.Trim());
                if (user == null || user.RefreshTokenExpiry <= DateTime.UtcNow)
                {
                    _logger.LogWarning("Invalid or expired refresh token for user ID: {UserId}", userId);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating refresh token for user ID {UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// Logs out a user by clearing their refresh token.
        /// </summary>
        public async Task<bool> LogoutUserAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), "User cannot be null.");

            user.RefreshToken = null;
            user.RefreshTokenExpiry = null;

            try
            {
                _context.Users.Update(user);
                return await _context.SaveChangesAsync() > 0;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error logging out user {Email}", user.Email);
                throw new InvalidOperationException("An error occurred while logging out the user.", ex);
            }
        }

        /// <summary>
        /// Retrieves a user by their ID.
        /// </summary>
        public async Task<User?> GetUserByIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));

            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    _logger.LogWarning("User not found for ID: {UserId}", userId);
                }
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user with ID: {UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// Generates a secure random refresh token.
        /// </summary>
        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes);
        }

         public async Task<User?> GetUserByRefreshTokenAsync(string refreshToken)
    {
        // ตรวจสอบว่า refreshToken เป็นค่า null หรือว่างหรือไม่
        if (string.IsNullOrEmpty(refreshToken))
        {
            return null;
        }

        // ค้นหาผู้ใช้ในฐานข้อมูลที่มีค่า refreshToken ตรงกับที่ระบุ
        var user = await _context.Users
                                  .Where(u => u.RefreshToken == refreshToken)
                                  .FirstOrDefaultAsync();

        // ส่งค่าผลลัพธ์กลับ
        return user;
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        // ตรวจสอบว่า email เป็นค่า null หรือว่างหรือไม่
        if (string.IsNullOrEmpty(email))
        {
            return null;
        }

        // ค้นหาผู้ใช้ในฐานข้อมูลที่มีค่า email ตรงกับที่ระบุ
        var user = await _context.Users
                                  .Where(u => u.Email == email)
                                  .FirstOrDefaultAsync();

        // ส่งค่าผลลัพธ์กลับ
        return user;
    }
    }
}
