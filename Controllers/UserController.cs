using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ForMiraiProject.Services.Interfaces;
using ForMiraiProject.ViewModels;
using System;
using System.Threading.Tasks;


namespace ForMiraiProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Requires JWT authentication
    public class UserController : ControllerBase
    {
        private readonly I_UserService _userService;
        private readonly ILogger<UserController> _logger;

        public UserController(I_UserService userService, ILogger<UserController> logger)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get user profile by authenticated user ID
        /// </summary>
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim))
                {
                    _logger.LogWarning("Invalid user identifier in claims.");
                    return Unauthorized(new { Message = "Invalid token or session expired." });
                }

                if (!Guid.TryParse(userIdClaim, out Guid userId))
                {
                    _logger.LogWarning("Invalid user ID format in token: {UserIdClaim}", userIdClaim);
                    return Unauthorized(new { Message = "Invalid token format." });
                }

                var userProfile = await _userService.GetUserProfileAsync(userId);
                if (userProfile == null)
                {
                    _logger.LogWarning("User profile not found for ID: {UserId}", userId);
                    return NotFound(new { Message = "User profile not found." });
                }

                return Ok(userProfile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching user profile.");
                return StatusCode(500, new { Message = "Internal Server Error." });
            }
        }

        /// <summary>
        /// Update user profile
        /// </summary>
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid update profile model state.");
                return BadRequest(ModelState);
            }

            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim))
                {
                    _logger.LogWarning("Invalid user identifier in claims.");
                    return Unauthorized(new { Message = "Invalid token or session expired." });
                }

                if (!Guid.TryParse(userIdClaim, out Guid userId))
                {
                    _logger.LogWarning("Invalid user ID format in token: {UserIdClaim}", userIdClaim);
                    return Unauthorized(new { Message = "Invalid token format." });
                }

                var updateResult = await _userService.UpdateUserProfileAsync(userId, model);

                if (!updateResult)
                {
                    _logger.LogWarning("Failed to update user profile for ID: {UserId}", userId);
                    return BadRequest(new { Message = "Failed to update user profile." });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating user profile.");
                return StatusCode(500, new { Message = "Internal Server Error." });
            }
        }

        /// <summary>
        /// Change user password
        /// </summary>
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid change password model state.");
                return BadRequest(ModelState);
            }

            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim))
                {
                    _logger.LogWarning("Invalid user identifier in claims.");
                    return Unauthorized(new { Message = "Invalid token or session expired." });
                }

                if (!Guid.TryParse(userIdClaim, out Guid userId))
                {
                    _logger.LogWarning("Invalid user ID format in token: {UserIdClaim}", userIdClaim);
                    return Unauthorized(new { Message = "Invalid token format." });
                }

                var changePasswordResult = await _userService.ChangeUserPasswordAsync(userId, model);

                if (!changePasswordResult)
                {
                    _logger.LogWarning("Failed to change password for user ID: {UserId}", userId);
                    return BadRequest(new { Message = "Failed to change password." });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while changing user password.");
                return StatusCode(500, new { Message = "Internal Server Error." });
            }
        }

        /// <summary>
        /// Delete user account
        /// </summary>
        [HttpDelete("delete-account")]
        public async Task<IActionResult> DeleteAccount()
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdClaim))
                {
                    _logger.LogWarning("Invalid user identifier in claims.");
                    return Unauthorized(new { Message = "Invalid token or session expired." });
                }

                if (!Guid.TryParse(userIdClaim, out Guid userId))
                {
                    _logger.LogWarning("Invalid user ID format in token: {UserIdClaim}", userIdClaim);
                    return Unauthorized(new { Message = "Invalid token format." });
                }

                var deleteResult = await _userService.DeleteUserAccountAsync(userId);

                if (!deleteResult)
                {
                    _logger.LogWarning("Failed to delete user account for ID: {UserId}", userId);
                    return BadRequest(new { Message = "Failed to delete account." });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting user account.");
                return StatusCode(500, new { Message = "Internal Server Error." });
            }
        }
    }
}
