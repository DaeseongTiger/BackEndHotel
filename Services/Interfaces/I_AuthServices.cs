using ForMiraiProject.ViewModels;
using System.Security.Claims;
using System.Threading.Tasks;
using ForMiraiProject.Models;

namespace ForMiraiProject.Services.Interfaces
{
    public interface I_AuthService
    {
        Task<RegisterResponseViewModel> RegisterAsync(RegisterViewModel model);
        Task<LoginResponseViewModel?> LoginAsync(LoginViewModel model);
        Task<string> RefreshTokenAsync(RefreshTokenViewModel model);
        Task<bool> RevokeRefreshTokenAsync(string token);
        Task<bool> ValidateJwtTokenAsync(string token);
        Task Logout(ClaimsPrincipal user);
        bool HasRole(ClaimsPrincipal user, string requiredRole);
        Task<string> GenerateCsrfTokenAsync();
        Task<bool> ValidateCsrfTokenAsync(string token);
        Task<User?> GetUserByIdAsync(Guid userId);

        

        

    }
}
