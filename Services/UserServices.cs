
using ForMiraiProject.Repositories.Interfaces;
using ForMiraiProject.Services.Interfaces;
using ForMiraiProject.ViewModels;
using UserModel = ForMiraiProject.Models.User;


namespace ForMiraiProject.Services
{
    public class UserService : I_UserService
    {
        private readonly I_UserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(I_UserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets the profile of the user by their ID.
        /// </summary>
        public async Task<UserProfileViewModel?> GetUserProfileAsync(Guid userId)
{
    if (userId == Guid.Empty)
    {
        _logger.LogWarning("Invalid user ID provided for profile retrieval.");
        throw new ArgumentException("User ID cannot be empty.", nameof(userId));
    }

    try
    {
        // ดึงข้อมูล user จาก repository
        var User = await _userRepository.GetUserByIdAsync(userId);
        if (User == null)
        {
            _logger.LogWarning("User not found for ID: {UserId}", userId);
            return null;
        }

        return new UserProfileViewModel
        {
            UserId = User.Id,
            FullName = User.FullName,
            Email = User.Email,
            PhoneNumber = User.PhoneNumber,
            DateOfBirth = User.DateOfBirth
        };
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "An error occurred while retrieving the user profile for ID: {UserId}", userId);
        throw;
    }
}

        /// <summary>
        /// Updates the user's profile information.
        /// </summary>
        public async Task<bool> UpdateUserProfileAsync(Guid userId, UpdateProfileViewModel model)
        {
            if (userId == Guid.Empty)
            {
                _logger.LogWarning("Invalid user ID provided for profile update.");
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }

            try
            {
                var user = await _userRepository.GetUserByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User not found for ID: {UserId}", userId);
                    return false;
                }

                user.FullName = model.FullName.Trim();
                user.PhoneNumber = model.PhoneNumber.Trim();
                user.DateOfBirth = model.DateOfBirth;

                return await _userRepository.UpdateUserAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the user profile for ID: {UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// Changes the user's password securely.
        /// </summary>
        public async Task<bool> ChangeUserPasswordAsync(Guid userId, ChangePasswordViewModel model)
        {
            if (userId == Guid.Empty)
            {
                _logger.LogWarning("Invalid user ID provided for password change.");
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }

            try
            {
                var user = await _userRepository.GetUserByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User not found for ID: {UserId}", userId);
                    return false;
                }

                if (!BCrypt.Net.BCrypt.Verify(model.OldPassword, user.PasswordHash))
                {
                    _logger.LogWarning("Incorrect old password provided for user ID: {UserId}", userId);
                    return false;
                }

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
                return await _userRepository.UpdateUserAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while changing the password for user ID: {UserId}", userId);
                throw;
            }
        }

        /// <summary>
        /// Deletes the user's account by their ID.
        /// </summary>
        public async Task<bool> DeleteUserAccountAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                _logger.LogWarning("Invalid user ID provided for account deletion.");
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }

            try
            {
                var user = await _userRepository.GetUserByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User not found for ID: {UserId}", userId);
                    return false;
                }

                return await _userRepository.DeleteUserAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the account for user ID: {UserId}", userId);
                throw;
            }
        }
    }
}
