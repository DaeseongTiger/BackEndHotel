using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ForMiraiProject.Models;
using ForMiraiProject.Services.Interfaces;
using ForMiraiProject.DTOs;
 // สำหรับ I_FeedbackService
using ForMiraiProject.Services;

namespace ForMiraiProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Requires authentication for all actions
    public class FeedbackController : ControllerBase
    {
        private readonly I_FeedbackService _feedbackService;
        private readonly ILogger<FeedbackController> _logger;

        public FeedbackController(I_FeedbackService feedbackService, ILogger<FeedbackController> logger)
        {
            _feedbackService = feedbackService ?? throw new ArgumentNullException(nameof(feedbackService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get all feedbacks.
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Admin")] // Only Admin can view all feedback
        public async Task<IActionResult> GetAllFeedbacks()
        {
            try
            {
                var feedbacks = await _feedbackService.GetAllFeedbacksAsync();
                return Ok(feedbacks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all feedbacks.");
                return StatusCode(500, "An error occurred while fetching feedbacks.");
            }
        }

        /// <summary>
        /// Get feedback by user ID.
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetFeedbackByUser(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest("Invalid user ID.");
            }

            try
            {
                var feedbacks = await _feedbackService.GetFeedbacksByUserIdAsync(userId);
                if (feedbacks == null)
                {
                    return NotFound("No feedback found for this user.");
                }
                return Ok(feedbacks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching feedback for user {UserId}.", userId);
                return StatusCode(500, "An error occurred while fetching feedback.");
            }
        }

        /// <summary>
        /// Get feedback by hotel ID.
        /// </summary>
        [HttpGet("hotel/{hotelId}")]
        public async Task<IActionResult> GetFeedbackByHotel(Guid hotelId)
        {
            if (hotelId == Guid.Empty)
            {
                return BadRequest("Invalid hotel ID.");
            }

            try
            {
                var feedbacks = await _feedbackService.GetFeedbacksByHotelIdAsync(hotelId);
                if (feedbacks == null)
                {
                    return NotFound("No feedback found for this hotel.");
                }
                return Ok(feedbacks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching feedback for hotel {HotelId}.", hotelId);
                return StatusCode(500, "An error occurred while fetching feedback.");
            }
        }

        /// <summary>
        /// Create new feedback.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddFeedback([FromBody] FeedbackCreateRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid feedback data.");
            }

            try
            {
                var feedback = await _feedbackService.CreateFeedbackAsync(request);
                return CreatedAtAction(nameof(GetFeedbackByUser), new { userId = feedback.UserId }, feedback);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Validation error while creating feedback.");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating feedback.");
                return StatusCode(500, "An error occurred while creating feedback.");
            }
        }

        /// <summary>
        /// Update existing feedback.
        /// </summary>
        [HttpPut("{id}")]
public async Task<IActionResult> UpdateFeedback(Guid id, [FromBody] FeedbackUpdateRequest request)
{
    if (id == Guid.Empty || request == null || id != request.FeedbackId)
    {
        return BadRequest("Invalid feedback data.");
    }

    try
    {
        var updatedFeedback = await _feedbackService.UpdateFeedbackAsync(id, request); // ส่ง id และ request
        if (updatedFeedback == null)
        {
            return NotFound("Feedback not found.");
        }
        return Ok(updatedFeedback);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error updating feedback with ID {Id}.", id);
        return StatusCode(500, "An error occurred while updating feedback.");
    }
}


        /// <summary>
        /// Delete feedback by ID.
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Only Admin can delete feedback
        public async Task<IActionResult> DeleteFeedback(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest("Invalid feedback ID.");
            }

            try
            {
                var result = await _feedbackService.DeleteFeedbackAsync(id);
                if (!result)
                {
                    return NotFound("Feedback not found.");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting feedback with ID {Id}.", id);
                return StatusCode(500, "An error occurred while deleting feedback.");
            }
        }
    }
}
