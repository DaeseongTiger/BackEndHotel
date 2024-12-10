using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ForMiraiProject.DTOs;
using ForMiraiProject.Models;
using ForMiraiProject.Repositories.Interfaces;
using ForMiraiProject.Services.Interfaces;

namespace ForMiraiProject.Services
{
    public class FeedbackService : I_FeedbackService
    {
        private readonly I_FeedbackRepository _feedbackRepository;
        private readonly I_HotelRepository _hotelRepository;
        private readonly I_UserRepository _userRepository;
        private readonly ILogger<FeedbackService> _logger;

        public FeedbackService(
            I_FeedbackRepository feedbackRepository,
            I_HotelRepository hotelRepository,
            I_UserRepository userRepository,
            ILogger<FeedbackService> logger)
        {
            _feedbackRepository = feedbackRepository ?? throw new ArgumentNullException(nameof(feedbackRepository));
            _hotelRepository = hotelRepository ?? throw new ArgumentNullException(nameof(hotelRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
        }

        public async Task<IEnumerable<Feedback>> GetAllFeedbacksAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all feedbacks.");
                return await _feedbackRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all feedbacks.");
                throw new InvalidOperationException("An error occurred while fetching feedbacks.", ex);
            }
        }

        public async Task<IEnumerable<Feedback>> GetFeedbacksByUserIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                _logger.LogWarning("Invalid user ID provided for fetching feedbacks.");
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }

            try
            {
                _logger.LogInformation("Fetching feedbacks for User ID {UserId}.", userId);
                return await _feedbackRepository.GetFeedbackByUserIdAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching feedbacks for User ID {UserId}.", userId);
                throw new InvalidOperationException("An error occurred while fetching feedbacks by user.", ex);
            }
        }

        public async Task<IEnumerable<Feedback>> GetFeedbacksByHotelIdAsync(Guid hotelId)
        {
            if (hotelId == Guid.Empty)
            {
                _logger.LogWarning("Invalid hotel ID provided for fetching feedbacks.");
                throw new ArgumentException("Hotel ID cannot be empty.", nameof(hotelId));
            }

            try
            {
                _logger.LogInformation("Fetching feedbacks for Hotel ID {HotelId}.", hotelId);
                return await _feedbackRepository.GetFeedbackByHotelIdAsync(hotelId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching feedbacks for Hotel ID {HotelId}.", hotelId);
                throw new InvalidOperationException("An error occurred while fetching feedbacks by hotel.", ex);
            }
        }

        public async Task<Feedback> CreateFeedbackAsync(FeedbackCreateRequest request)
{
    if (request == null || string.IsNullOrWhiteSpace(request.Content))
    {
        _logger.LogWarning("Invalid feedback creation request.");
        throw new ArgumentNullException(nameof(request), "Request or content cannot be null.");
    }

    if (request.Rating < 1 || request.Rating > 5)
    {
        _logger.LogWarning("Invalid rating: {Rating}.", request.Rating);
        throw new ArgumentException("Rating must be between 1 and 5.");
    }

    try
    {
        var userExists = await _userRepository.ExistsAsync(request.UserId);
        if (!userExists)
            throw new InvalidOperationException($"User ID {request.UserId} does not exist.");

        var hotelExists = await _hotelRepository.ExistsAsync(request.HotelId);
        if (!hotelExists)
            throw new InvalidOperationException($"Hotel ID {request.HotelId} does not exist.");

        var feedback = new Feedback
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            HotelId = request.HotelId,
            Content = request.Content.Trim(),
            Rating = request.Rating,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _feedbackRepository.AddAsync(feedback);
        _logger.LogInformation("Feedback created for User ID {UserId}, Hotel ID {HotelId}.", request.UserId, request.HotelId);

        return feedback;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error creating feedback.");
        throw;
    }
}


        public async Task<Feedback?> UpdateFeedbackAsync(Guid id, FeedbackUpdateRequest request)
{
    if (id == Guid.Empty || request == null || id != request.FeedbackId)
    {
        throw new ArgumentException("Invalid feedback update data.");
    }

    try
    {
        var existingFeedback = await _feedbackRepository.GetByIdAsync(id);
        if (existingFeedback == null)
        {
            return null;
        }

        // ใช้ Method สำหรับการอัปเดตค่า
        existingFeedback.UpdateRating(request.Rating);
        existingFeedback.UpdateContent(request.Content);

        existingFeedback.UpdatedAt = DateTime.UtcNow;

        await _feedbackRepository.UpdateAsync(existingFeedback);
        return existingFeedback;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error updating feedback with ID {Id}.", id);
        throw new InvalidOperationException("An error occurred while updating feedback.", ex);
    }
}



        public async Task<bool> DeleteFeedbackAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                _logger.LogWarning("Invalid feedback ID provided for deletion.");
                throw new ArgumentException("Feedback ID cannot be empty.", nameof(id));
            }

            try
            {
                var feedback = await _feedbackRepository.GetByIdAsync(id);
                if (feedback == null)
                {
                    _logger.LogWarning("Feedback with ID {Id} not found.", id);
                    return false;
                }

                await _feedbackRepository.DeleteAsync(id);
                _logger.LogInformation("Feedback with ID {Id} successfully deleted.", id);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting feedback with ID {Id}.", id);
                throw new InvalidOperationException("An error occurred while deleting feedback.", ex);
            }
        }
        public async Task<IEnumerable<Feedback>> GetAllFeedbackAsync()
{
    try
    {
        _logger.LogInformation("Fetching all feedback entries.");
        return await _feedbackRepository.GetAllAsync();
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error fetching all feedback entries.");
        throw;
    }
}

public async Task<Feedback?> GetFeedbackByIdAsync(Guid id)
{
    if (id == Guid.Empty)
    {
        _logger.LogWarning("Invalid feedback ID provided.");
        throw new ArgumentException("Feedback ID cannot be empty.", nameof(id));
    }

    try
    {
        _logger.LogInformation("Fetching feedback with ID {Id}.", id);
        return await _feedbackRepository.GetByIdAsync(id);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error fetching feedback with ID {Id}.", id);
        throw;
    }
}
        
    }
}
