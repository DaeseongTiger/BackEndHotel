using ForMiraiProject.ViewModels;
using System.Security.Claims;
using System.Threading.Tasks;
using ForMiraiProject.Data;

namespace ForMiraiProject.Services.Interfaces
{
    /// <summary>
    /// Interface for authentication services
    /// </summary>
    public interface I_AuthService
    {
        /// <summary>
        /// Registers a new user in the system.
        /// </summary>
        /// <param name="model">The registration data.</param>
        /// <returns>Details of the registered user.</returns>
        Task<RegisterResponseViewModel> RegisterAsync(RegisterViewModel model);

        /// <summary>
        /// Authenticates a user and generates a JWT token.
        /// </summary>
        /// <param name="model">The login data.</param>
        /// <returns>The login response with a JWT token and its expiry date, or null if authentication fails.</returns>
        Task<LoginResponseViewModel?> LoginAsync(LoginViewModel model);

        /// <summary>
        /// Refreshes an expired or about-to-expire JWT token using a refresh token.
        /// </summary>
        /// <param name="model">The refresh token data.</param>
        /// <returns>A new JWT token and its expiry date, or null if the refresh token is invalid or expired.</returns>
        Task<TokenResponseViewModel?> RefreshTokenAsync(RefreshTokenViewModel model);

        /// <summary>
        /// Logs the user out by invalidating their refresh token.
        /// </summary>
        /// <param name="user">The currently authenticated user.</param>
        void Logout(ClaimsPrincipal user);
    }
}
