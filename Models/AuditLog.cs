using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForMiraiProject.Models
{
    // AuditLog model สำหรับการเก็บข้อมูลการตรวจสอบการเปลี่ยนแปลง
    public class AuditLog
    {
        [Key]
        public Guid Id { get; set; } // รหัส AuditLog

        public Guid UserId { get; set; } // ผู้ใช้ที่ทำการกระทำ

        [Required]
        [MaxLength(200)]
        public  string? Action { get; set; } // การกระทำที่เกิดขึ้น (เช่น Create, Update, Delete)

        [Required]
        [MaxLength(100)]
        public  string? Entity { get; set; } // Entity ที่กระทบ (เช่น User, Booking)

        [Required]
        [MaxLength(500)]
        public  string? Changes { get; set; } // รายละเอียดการเปลี่ยนแปลง

        [Required]
        public DateTime Timestamp { get; set; } // เวลาในการกระทำ
        public required string ActionType { get; set; }

    public required string Description { get; set; }
    public required string NewValue { get; set; }
    public required string OldValue { get; set; }
    public  required string Status { get; set; }
    public DateTime CreatedAt { get; set; }


        // ตัวแปรที่ใช้ในการจัดเก็บเหตุการณ์ที่เกิดขึ้นในระบบ
        [NotMapped]
        public string FormattedMessage
        {
            get
            {
                return $"{Action} on {Entity} at {Timestamp}: {Changes}";
            }
        }

        // ตัวสร้าง default สำหรับการสร้าง AuditLog
        public AuditLog()
        {
            Timestamp = DateTime.UtcNow; // กำหนดค่าเวลาปัจจุบัน
        }
    }
}
