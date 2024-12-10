using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ForMiraiProject.Models;
using ForMiraiProject.DTOs;

namespace ForMiraiProject.Services.Interfaces
{
    /// <summary>
    /// Interface for feedback service to handle feedback-related operations.
    /// </summary>
    public interface I_FeedbackService
    {
        /// <summary>
        /// Retrieve all feedback entries.
        /// </summary>
        /// <returns>A list of feedback entries.</returns>
        Task<IEnumerable<Feedback>> GetAllFeedbackAsync();

        /// <summary>
        /// Retrieve feedback by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the feedback.</param>
        /// <returns>The feedback entry if found; otherwise, null.</returns>
        Task<Feedback?> GetFeedbackByIdAsync(Guid id);

        /// <summary>
        /// Create a new feedback entry.
        /// </summary>
        /// <param name="request">The feedback creation request.</param>
        /// <returns>The newly created feedback entry.</returns>
        Task<Feedback> CreateFeedbackAsync(FeedbackCreateRequest request);

        /// <summary>
        /// Update an existing feedback entry.
        /// </summary>
        /// <param name="id">The unique identifier of the feedback to update.</param>
        /// <param name="request">The feedback update request.</param>
        /// <returns>The updated feedback entry if successful; otherwise, null.</returns>
        Task<Feedback?> UpdateFeedbackAsync(Guid id, FeedbackUpdateRequest request);

        /// <summary>
        /// Delete a feedback entry.
        /// </summary>
        /// <param name="id">The unique identifier of the feedback to delete.</param>
        /// <returns>True if the feedback was deleted successfully; otherwise, false.</returns>
        Task<bool> DeleteFeedbackAsync(Guid id);

        Task<IEnumerable<Feedback>> GetAllFeedbacksAsync();
        Task<IEnumerable<Feedback>> GetFeedbacksByUserIdAsync(Guid userId);
        Task<IEnumerable<Feedback>> GetFeedbacksByHotelIdAsync(Guid hotelId);
        
        
    }
}
