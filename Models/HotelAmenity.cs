using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace ForMiraiProject.Models
{
    public class HotelAmenity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } // Primary Key

        [Required(ErrorMessage = "Amenity name is required")]
        [MaxLength(100, ErrorMessage = "Amenity name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty; // ชื่อสิ่งอำนวยความสะดวก

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; } = string.Empty; // คำอธิบายของสิ่งอำนวยความสะดวก

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // วันที่สร้าง

        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // วันที่แก้ไขล่าสุด

        [Required]
        public bool IsActive { get; set; } = true; // สถานะของสิ่งอำนวยความสะดวก

        // ฟังก์ชันช่วย
        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateDetails(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Amenity name cannot be empty or null.");
            
            if (name.Length > 100)
                throw new ArgumentException("Amenity name cannot exceed 100 characters.");

            if (description.Length > 500)
                throw new ArgumentException("Description cannot exceed 500 characters.");

            Name = WebUtility.HtmlEncode(name.Trim());
            Description = WebUtility.HtmlEncode(description.Trim());
            UpdatedAt = DateTime.UtcNow;
        }

        public static bool ValidateIsActive(bool isActive)
        {
            return isActive == true || isActive == false;
        }
    }
}
