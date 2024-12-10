using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace ForMiraiProject.Models
{
    public class Feedback
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required]
        public virtual User User { get; set; } = null!;

        [Required]
        public Guid HotelId { get; set; }

        

        [Required]
        public virtual Hotel Hotel { get; set; } = null!;

        [Required]
        [MaxLength(1000, ErrorMessage = "Content cannot exceed 1000 characters.")]
        public string Content { get;  set; } = string.Empty;

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public bool IsActive { get; set; } = true;

        public Guid CreatedBy { get; set; }
        public Guid? LastUpdatedBy { get; set; }


        public void SetContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ValidationException("Feedback content cannot be empty.");

            Content = WebUtility.HtmlEncode(content.Trim());
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateFeedback(string content, int rating)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ValidationException("Feedback content cannot be empty.");

            if (rating < 1 || rating > 5)
                throw new ValidationException("Rating must be between 1 and 5.");

            Content = WebUtility.HtmlEncode(content.Trim());
            Rating = rating;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ValidateUserOwnership(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));

            if (userId != UserId)
                throw new UnauthorizedAccessException("You are not authorized to update this feedback.");
        }

        public bool CanEditFeedback()
        {
            return DateTime.UtcNow - CreatedAt <= TimeSpan.FromDays(7);
        }

        public void MarkAsInactive()
        {
            IsActive = false;
            Content = "This feedback has been removed.";
            Rating = 0;
            UpdatedAt = DateTime.UtcNow;
        }

   public void UpdateRating(int newRating)
    {
        if (newRating < 1 || newRating > 5)
            throw new ArgumentException("Rating must be between 1 and 5.", nameof(newRating));

        Rating = newRating;
    }

   public void UpdateContent(string newContent)
    {
        if (string.IsNullOrWhiteSpace(newContent))
            throw new ArgumentException("Content cannot be empty.", nameof(newContent));

        Content = newContent.Trim();
    }
    }
}
