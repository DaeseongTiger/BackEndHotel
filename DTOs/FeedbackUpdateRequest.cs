using System;
using System.ComponentModel.DataAnnotations;

namespace ForMiraiProject.DTOs
{
    public class FeedbackUpdateRequest
    {
    [Required]
    public Guid FeedbackId { get; set; }

    [Required]
    [MaxLength(1000)]
    public string Content { get; set; } = string.Empty;

    [Required]
    [Range(1, 5)]
    public int Rating { get; set; }
        
    }
}
