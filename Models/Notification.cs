using ForMiraiProject.ViewModels;
namespace ForMiraiProject.Models
{
    using System;

    public class Notification
    {
        public Guid NotificationId { get; set; }
        public Guid UserId { get; set; }
        public string? Message { get; set; }
        public NotificationType NotificationType { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsDelivered { get; set; }

        // เพิ่มข้อมูลอื่น ๆ ที่อาจจะใช้ เช่น status, error message เป็นต้น
    }
}
