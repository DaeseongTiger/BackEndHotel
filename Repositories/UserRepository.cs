using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ForMiraiProject.Data;
using ForMiraiProject.Models;
using ForMiraiProject.Repositories.Interfaces;

namespace ForMiraiProject.Repositories
{
    public class UserRepository : GenericRepository<User>, I_UserRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(AppDbContext context, ILogger<UserRepository> logger)
            : base(context, logger as ILogger<GenericRepository<User>>)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// ดึงข้อมูลผู้ใช้ตามอีเมล
        /// </summary>
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                _logger.LogWarning("Attempted to search user with an empty email.");
                throw new ArgumentException("Email cannot be empty or null.", nameof(email));
            }

            email = email.Trim().ToLower();

            try
            {
                var user = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == email);

                if (user == null)
                {
                    _logger.LogWarning("User with email {Email} not found.", email);
                }
                else
                {
                    _logger.LogInformation("User with email {Email} fetched successfully.", email);
                }

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the user by email: {Email}", email);
                throw;
            }
        }

        /// <summary>
        /// ดึงข้อมูลผู้ใช้ตาม ID
        /// </summary>
        public async Task<User?> GetUserByIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                _logger.LogWarning("Invalid user ID provided for retrieval.");
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }

            try
            {
                return await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the user by ID: {UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// ค้นหาผู้ใช้ตามบทบาท
        /// </summary>
        public async Task<IEnumerable<User>> FindUsersByRoleAsync(string role)
        {
            if (string.IsNullOrWhiteSpace(role))
            {
                _logger.LogWarning("Attempted to search users with an empty role.");
                throw new ArgumentException("Role cannot be empty or null.", nameof(role));
            }

            role = role.Trim().ToLower();

            try
            {
                var users = await _context.Users
                    .Where(u => u.Role.ToLower() == role)
                    .AsNoTracking()
                    .ToListAsync();

                if (!users.Any())
                {
                    _logger.LogWarning("No users found with role {Role}.", role);
                }
                else
                {
                    _logger.LogInformation("{Count} users found with role {Role}.", users.Count, role);
                }

                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching users by role: {Role}", role);
                throw;
            }
        }

        /// <summary>
        /// ค้นหาผู้ใช้ตามบทบาท พร้อมรองรับ Paging
        /// </summary>
        public async Task<IEnumerable<User>> FindUsersByRoleAsync(string role, int pageIndex, int pageSize)
        {
            if (string.IsNullOrWhiteSpace(role))
            {
                _logger.LogWarning("Attempted to search users with an empty role.");
                throw new ArgumentException("Role cannot be empty or null.", nameof(role));
            }

            if (pageIndex < 0 || pageSize <= 0)
            {
                _logger.LogWarning("Invalid pagination parameters provided.");
                throw new ArgumentException("Invalid pagination parameters.");
            }

            role = role.Trim().ToLower();

            try
            {
                var users = await _context.Users
                    .Where(u => u.Role.ToLower() == role)
                    .AsNoTracking()
                    .Skip(pageIndex * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                if (!users.Any())
                {
                    _logger.LogWarning("No users found with role {Role} for the specified page.", role);
                }
                else
                {
                    _logger.LogInformation("{Count} users found with role {Role} for the specified page.", users.Count, role);
                }

                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching users by role with pagination. Role: {Role}, PageIndex: {PageIndex}, PageSize: {PageSize}", role, pageIndex, pageSize);
                throw;
            }
        }

        /// <summary>
        /// อัปเดตข้อมูลของผู้ใช้
        /// </summary>
        public async Task<bool> UpdateUserAsync(User user)
        {
            if (user == null)
            {
                _logger.LogWarning("Attempted to update a null user.");
                throw new ArgumentNullException(nameof(user), "User cannot be null.");
            }

            try
            {
                _context.Users.Update(user);
                var updated = await _context.SaveChangesAsync() > 0;
                if (updated)
                {
                    _logger.LogInformation("User with ID {UserId} updated successfully.", user.Id);
                }
                return updated;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the user with ID: {UserId}", user.Id);
                throw;
            }
        }

        /// <summary>
        /// ลบผู้ใช้
        /// </summary>
        public async Task<bool> DeleteUserAsync(User user)
        {
            if (user == null)
            {
                _logger.LogWarning("Attempted to delete a null user.");
                throw new ArgumentNullException(nameof(user), "User cannot be null.");
            }

            try
            {
                _context.Users.Remove(user);
                var deleted = await _context.SaveChangesAsync() > 0;
                if (deleted)
                {
                    _logger.LogInformation("User with ID {UserId} deleted successfully.", user.Id);
                }
                return deleted;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the user with ID: {UserId}", user.Id);
                throw;
            }
        }

        /// <summary>
        /// ค้นหาผู้ใช้ตามคำค้นหา (Search Term)
        /// </summary>
        public async Task<IEnumerable<User>> SearchUsersAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                _logger.LogWarning("Empty search term provided for user search.");
                throw new ArgumentException("Search term cannot be empty or null.", nameof(searchTerm));
            }

            try
            {
                return await _context.Users
                    .Where(u => u.FullName.Contains(searchTerm) || u.Email.Contains(searchTerm))
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while searching users with term: {SearchTerm}", searchTerm);
                throw;
            }
        }

        /// <summary>
        /// ตรวจสอบว่าผู้ใช้อีเมลนี้มีอยู่ในระบบหรือไม่
        /// </summary>
        public async Task<bool> EmailExistsAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                _logger.LogWarning("Empty email provided for email existence check.");
                throw new ArgumentException("Email cannot be empty or null.", nameof(email));
            }

            try
            {
                return await _context.Users.AnyAsync(u => u.Email.ToLower() == email.Trim().ToLower());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while checking email existence: {Email}", email);
                throw;
            }
        }
    }
}
