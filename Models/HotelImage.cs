using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace ForMiraiProject.Models
{
    public class HotelImage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } // Primary Key

        [Required(ErrorMessage = "Image URL is required")]
        [Url(ErrorMessage = "Invalid URL format")]
        [MaxLength(500, ErrorMessage = "URL cannot exceed 500 characters.")]
        public string ImageUrl { get; set; } = string.Empty; // URL ของภาพโรงแรม

        [Required(ErrorMessage = "Hotel Id is required")]
        public Guid HotelId { get; set; } // Foreign Key ไปยังโรงแรม

        public Hotel? Hotel { get; set; } // ความสัมพันธ์กับ Hotel (nullable)

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // วันที่สร้าง

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // วันที่แก้ไขล่าสุด

        [Required]
        public bool IsActive { get; set; } = true; // สถานะของภาพ (ใช้งานหรือไม่)

        // ฟังก์ชันช่วยในการอัปเดตภาพ
        public void UpdateImage(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                throw new ArgumentException("Image URL cannot be empty or null.", nameof(imageUrl));

            if (!IsValidImageUrl(imageUrl))
                throw new ArgumentException("Invalid image URL format.", nameof(imageUrl));

            // ป้องกัน XSS และการทำงานไม่ถูกต้องจาก URL
            ImageUrl = WebUtility.HtmlEncode(imageUrl.Trim());
            UpdatedAt = DateTime.UtcNow;
        }

        // ฟังก์ชันที่ใช้สำหรับการตรวจสอบ URL
        public static bool IsValidImageUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult) &&
                   (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        // ฟังก์ชันเพื่อให้สามารถปิดการใช้งานภาพได้
        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        // ตรวจสอบค่าของสถานะ IsActive
        public static bool ValidateIsActive(bool isActive)
        {
            return isActive == true || isActive == false;
        }

        // ฟังก์ชันตรวจสอบความสัมพันธ์
        public void ValidateHotelRelationship(Guid hotelId)
        {
            if (hotelId == Guid.Empty)
                throw new ArgumentException("Hotel ID cannot be empty or invalid.", nameof(hotelId));
        }
    }
}
