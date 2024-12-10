using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace ForMiraiProject.Models
{
    public class Review
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } // Primary Key

        [Required]
        public Guid UserId { get; set; } // Foreign Key to User

        [Required]
        public User? User { get; set; }  // Navigation property for User

        [Required]
        public Guid HotelId { get; set; }
        
        [Required] // Foreign Key to Hotel
        public Hotel? Hotel { get; set; }  // Navigation property for Hotel

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; } // Rating between 1 and 5

        [Required]
        [MaxLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters.")]
        public string Comment { get; set; } = string.Empty; // Comment content (non-nullable)

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Review creation time
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // Last updated time

        // Constructor to set Comment content and encode to prevent XSS
        public void SetComment(string comment)
        {
            if (string.IsNullOrWhiteSpace(comment))
                throw new ValidationException("Comment cannot be empty.");

            // HTML encode to prevent XSS
            Comment = WebUtility.HtmlEncode(comment.Trim());
            UpdatedAt = DateTime.UtcNow;
        }

        // Method to update review with validation
        public void UpdateReview(string comment, int rating)
        {
            if (string.IsNullOrWhiteSpace(comment))
                throw new ValidationException("Comment cannot be empty.");

            if (rating < 1 || rating > 5)
                throw new ValidationException("Rating must be between 1 and 5.");

            Comment = WebUtility.HtmlEncode(comment.Trim()); // Encoding comment to prevent XSS
            Rating = rating;
            UpdatedAt = DateTime.UtcNow;
        }

        // Method to validate if the user is the owner of the review
        public void ValidateOwnership(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty.");

            if (userId != UserId)
                throw new UnauthorizedAccessException("You are not authorized to edit this review.");
        }

        // Check if review can be edited (e.g. within 7 days of creation)
        public bool CanEdit()
        {
            return DateTime.UtcNow - CreatedAt <= TimeSpan.FromDays(7);
        }

        // Mark the review as inactive (soft delete)
        public void MarkAsInactive()
        {
            Rating = 0; // Optional: Set rating to 0 to indicate it's inactive
            Comment = "This review has been removed."; // Mask the content
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
