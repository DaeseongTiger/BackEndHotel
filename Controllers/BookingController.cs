using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ForMiraiProject.Services.Interfaces;
using ForMiraiProject.ViewModels;
using ForMiraiProject.Utilities;
using System;
using System.Threading.Tasks;

namespace ForMiraiProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase
    {
        private readonly I_BookingService _bookingService;
        private readonly ILogger<BookingController> _logger;

        public BookingController(I_BookingService bookingService, ILogger<BookingController> logger)
        {
            _bookingService = bookingService ?? throw new ArgumentNullException(nameof(bookingService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Create a new booking.
        /// </summary>
        /// <param name="bookingRequest">Booking details.</param>
        /// <returns>Operation result with booking information.</returns>
        [HttpPost("create")]
        [Authorize]  // Requires authentication
        public async Task<IActionResult> CreateBooking([FromBody] BookingRequestViewModel bookingRequest)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid booking request.");
                return BadRequest(ModelState);
            }

            try
{
    var result = await _bookingService.CreateBookingAsync(bookingRequest);
    if (result.IsSuccess)
    {
        return CreatedAtAction(nameof(GetBookingById), new { bookingId = result.ResultData }, result); // ใช้ ResultData แทน Data
    }
    return BadRequest(new { Message = result.Message });
}
catch (Exception ex)
{
    _logger.LogError(ex, "Error occurred while creating booking.");
    return StatusCode(500, new { Message = "Internal server error." });
}
        }

        /// <summary>
        /// Get a booking by ID.
        /// </summary>
        /// <param name="bookingId">Booking ID</param>
        /// <returns>Booking details.</returns>
        [HttpGet("{bookingId}")]
        [Authorize]  // Requires authentication
        public async Task<IActionResult> GetBookingById(Guid bookingId)
        {
            try
            {
                var booking = await _bookingService.GetBookingByIdAsync(bookingId);
                if (booking == null)
                {
                    _logger.LogWarning("Booking not found for ID: {BookingId}", bookingId);
                    return NotFound(new { Message = "Booking not found." });
                }
                return Ok(booking);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching booking details.");
                return StatusCode(500, new { Message = "Internal server error." });
            }
        }

        /// <summary>
        /// Cancel a booking.
        /// </summary>
        /// <param name="bookingId">Booking ID</param>
        /// <returns>Operation result.</returns>
        [HttpDelete("cancel/{bookingId}")]
        [Authorize]  // Requires authentication
        public async Task<IActionResult> CancelBooking(Guid bookingId)
        {
            try
            {
                var result = await _bookingService.CancelBookingAsync(bookingId);
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                return BadRequest(new { Message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while canceling booking.");
                return StatusCode(500, new { Message = "Internal server error." });
            }
        }

        /// <summary>
        /// Get all bookings for a user.
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of bookings.</returns>
        [HttpGet("user/{userId}")]
        [Authorize]  // Requires authentication
        public async Task<IActionResult> GetBookingsByUser(Guid userId)
{
    try
    {
        var bookings = await _bookingService.GetBookingsByUserAsync(userId);
        if (bookings == null || !bookings.Any())
        {
            _logger.LogWarning("No bookings found for user ID: {UserId}", userId);
            return NotFound(new { Message = "No bookings found." });
        }
        return Ok(bookings);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error occurred while fetching bookings for user.");
        return StatusCode(500, new { Message = "Internal server error." });
    }
}



        /// <summary>
        /// Update the booking status.
        /// </summary>
        /// <param name="bookingId">Booking ID</param>
        /// <param name="status">New status</param>
        /// <returns>Operation result.</returns>
        [HttpPut("update-status/{bookingId}")]
        [Authorize]  // Requires authentication
        public async Task<IActionResult> UpdateBookingStatus(Guid bookingId, [FromBody] string status)
        {
            if (string.IsNullOrEmpty(status))
            {
                return BadRequest(new { Message = "Status is required." });
            }

            try
            {
                var result = await _bookingService.UpdateBookingStatusAsync(bookingId, status);
                if (result.IsSuccess)
                {
                    return Ok(result);
                }
                return BadRequest(new { Message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating booking status.");
                return StatusCode(500, new { Message = "Internal server error." });
            }
        }
    }
}
