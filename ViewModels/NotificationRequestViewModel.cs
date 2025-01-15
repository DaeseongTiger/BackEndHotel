namespace ForMiraiProject.ViewModels

{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class NotificationRequestViewModel
    {
        [Required]
        public Guid UserId { get; set; }

        
        [StringLength(500, ErrorMessage = "Message cannot exceed 500 characters.")]

        
        public string Message { get; set; } = string.Empty;

        [Required]
        public NotificationType NotificationType { get; set; } // e.g., Email, SMS, Push
    }

    // Enum to define notification types
    public enum NotificationType
    {
        Email,
        SMS,
        Push
    }
}