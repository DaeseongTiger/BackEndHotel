using System;
using System.ComponentModel.DataAnnotations;

namespace ForMiraiProject.DTOs
{
    public class FeedbackCreateRequest
    {
        [Required]
        public Guid UserId { get; set; }

        [Required]
        public Guid HotelId { get; set; }

        [Required]
        [MaxLength(1000, ErrorMessage = "Content cannot exceed 1000 characters.")]
        public string Content { get; set; } = string.Empty;

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }
    }
}
