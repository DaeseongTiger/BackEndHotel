using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using ForMiraiProject.Models;
using ForMiraiProject.Services.Interfaces;
using ForMiraiProject.ViewModels;
using ForMiraiProject.DTOs; 

namespace ForMiraiProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookingController : ControllerBase
    {
        private readonly I_BookingService _bookingService;
        private readonly ILogger<BookingController> _logger;

        public BookingController(I_BookingService bookingService, ILogger<BookingController> logger)
        {
            _bookingService = bookingService;
            _logger = logger;
        }

        // สร้างการจองใหม่
        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreateBookingAsync([FromBody] Booking booking)
        {
            if (booking == null)
            {
                _logger.LogWarning("Invalid booking attempt: booking object is null.");
                return BadRequest("Booking details cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(booking.UserName) || booking.CheckInDate == default)
            {
                return BadRequest("Invalid booking data provided.");
            }

            try
            {
                var bookingRequestViewModel = new BookingRequestViewModel(booking.UserName)
{
    UserId = booking.UserId,
    RoomId = booking.RoomId,
    StartDate = booking.CheckInDate,
    EndDate = booking.CheckOutDate,
    SpecialRequests = booking.SpecialRequests
};

                await _bookingService.CreateBookingAsync(bookingRequestViewModel);
                
                return Ok(new { Message = "Booking successfully created" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating the booking.");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }

        // ดูข้อมูลการจอง
        [HttpGet]
[Route("details/{bookingId}")]
public async Task<IActionResult> GetBookingDetailsAsync(Guid bookingId)
{
    if (bookingId == Guid.Empty)
    {
        return BadRequest("Invalid booking ID.");
    }

    try
    {
        var booking = await _bookingService.GetBookingByIdAsync(bookingId); // ใช้ Guid
        if (booking == null)
        {
            return NotFound("Booking not found.");
        }

        return Ok(booking);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, $"Error occurred while fetching booking details for ID: {bookingId}");
        return StatusCode(500, "Internal server error.");
    }
}


        // ลบการจอง
        [HttpDelete]
        [Route("delete/{bookingId}")]
        public async Task<IActionResult> DeleteBookingAsync(Guid bookingId)
        {
            if (bookingId == Guid.Empty)
            {
                return BadRequest("Invalid booking ID.");
            }

            try
            {
                var result = await _bookingService.DeleteBookingAsync(bookingId);
                if (result)
                {
                    _logger.LogInformation($"Booking with ID: {bookingId} has been successfully deleted.");
                    return Ok(new { Message = "Booking deleted successfully." });
                }

                return NotFound("Booking not found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting booking with ID: {bookingId}");
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}
