using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ForMiraiProject.Models;
using ForMiraiProject.Services.Interfaces;
using ForMiraiProject.ViewModels;
using System;
using System.Threading.Tasks;
using System.Security.Claims;

namespace ForMiraiProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly I_AuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(I_AuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel? model)
        {
            if (model == null || !ModelState.IsValid)
            {
                _logger.LogWarning("Invalid registration model state.");
                return BadRequest(new { Success = false, Message = "Invalid input.", Errors = ModelState });
            }

            try
            {
                var result = await _authService.RegisterAsync(model);
                return CreatedAtAction(nameof(Register), new { id = result.UserId }, new
                {
                    Success = true,
                    Message = "Registration successful.",
                    Data = result
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Registration failed.");
                return BadRequest(new { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during registration.");
                return StatusCode(500, new { Success = false, Message = "Internal Server Error." });
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginViewModel? model)
        {
            if (model == null || !ModelState.IsValid)
            {
                _logger.LogWarning("Invalid login model state.");
                return BadRequest(new { Success = false, Message = "Invalid input.", Errors = ModelState });
            }

            try
            {
                var token = await _authService.LoginAsync(model);
                if (token == null)
                {
                    _logger.LogWarning("Invalid login attempt for email: {Email}", model.Email);
                    return Unauthorized(new { Success = false, Message = "Invalid credentials." });
                }

                return Ok(new { Success = true, Message = "Login successful.", Data = token });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized login attempt.");
                return Unauthorized(new { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login.");
                return StatusCode(500, new { Success = false, Message = "Internal Server Error." });
            }
        }

        [HttpPost("refresh-token")]
        [Authorize]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenViewModel? model)
        {
            if (model == null || !ModelState.IsValid)
            {
                _logger.LogWarning("Invalid refresh token model state.");
                return BadRequest(new { Success = false, Message = "Invalid input.", Errors = ModelState });
            }

            try
            {
                var token = await _authService.RefreshTokenAsync(model);
                if (token == null)
                {
                    _logger.LogWarning("Invalid or expired refresh token attempt.");
                    return Unauthorized(new { Success = false, Message = "Invalid or expired refresh token." });
                }

                return Ok(new { Success = true, Message = "Token refreshed successfully.", Data = token });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized refresh token attempt.");
                return Unauthorized(new { Success = false, Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during token refresh.");
                return StatusCode(500, new { Success = false, Message = "Internal Server Error." });
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("Logout failed: Invalid user identifier.");
                    return BadRequest(new { Success = false, Message = "Invalid user identifier." });
                }

                _authService.Logout(User);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during logout.");
                return StatusCode(500, new { Success = false, Message = "Internal Server Error." });
            }
        }
        
    }
}
