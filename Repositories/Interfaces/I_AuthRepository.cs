using ForMiraiProject.Models;
using System;
using System.Threading.Tasks;

namespace ForMiraiProject.Repositories.Interfaces
{
    public interface I_AuthRepository
    {
        /// <summary>
        /// Retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>The user object if found; otherwise, null.</returns>
        Task<User?> GetUserByIdAsync(Guid userId);

        /// <summary>
        /// Registers a new user into the system with validation.
        /// </summary>
        /// <param name="user">The user object to register.</param>
        /// <returns>True if registration is successful; otherwise, false.</returns>
        Task<bool> RegisterUserAsync(User user);

        /// <summary>
        /// Validates user login by checking the provided credentials.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <param name="password">The user's password.</param>
        /// <returns>A User object if login is successful; otherwise, null.</returns>
        Task<User?> LoginUserAsync(string email, string password);

        /// <summary>
        /// Updates the refresh token for the user after a successful login.
        /// </summary>
        /// <param name="user">The user object whose token is being updated.</param>
        /// <returns>True if the update is successful; otherwise, false.</returns>
        Task<bool> UpdateRefreshTokenAsync(User user);

        /// <summary>
        /// Validates if the refresh token is valid and not expired.
        /// </summary>
        /// <param name="userId">The user's unique identifier.</param>
        /// <param name="refreshToken">The refresh token to validate.</param>
        /// <returns>True if the token is valid and not expired; otherwise, false.</returns>
        Task<bool> ValidateRefreshTokenAsync(Guid userId, string refreshToken);

        /// <summary>
        /// Logs out the user by clearing their refresh token or session data.
        /// </summary>
        /// <param name="user">The user object whose session is being cleared.</param>
        /// <returns>True if logout is successful; otherwise, false.</returns>
        Task<bool> LogoutUserAsync(User user);
        Task<User?> GetUserByRefreshTokenAsync(string refreshToken);

        Task<User?> GetUserByEmailAsync(string email);

    }
}
