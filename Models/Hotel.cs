using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace ForMiraiProject.Models
{
    public class Hotel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } // Primary Key

        [Required(ErrorMessage = "Hotel name is required")]
        [MaxLength(100, ErrorMessage = "Hotel name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty; // ชื่อโรงแรม

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; } = string.Empty; // รายละเอียดเกี่ยวกับโรงแรม

        [Required(ErrorMessage = "Address is required")]
        [MaxLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string Address { get; set; } = string.Empty; // ที่อยู่

        [Required(ErrorMessage = "City is required")]
        [MaxLength(50, ErrorMessage = "City cannot exceed 50 characters.")]
        public string City { get; set; } = string.Empty; // เมือง

        [Required(ErrorMessage = "Country is required")]
        [MaxLength(50, ErrorMessage = "Country cannot exceed 50 characters.")]
        public string Country { get; set; } = string.Empty; // ประเทศ

        [Required(ErrorMessage = "ZipCode is required")]
        [MaxLength(10, ErrorMessage = "ZipCode cannot exceed 10 characters.")]
        public string ZipCode { get; set; } = string.Empty; // รหัสไปรษณีย์

        [MaxLength(15, ErrorMessage = "Phone number cannot exceed 15 characters.")]
        public string Phone { get; set; } = string.Empty; // เบอร์โทรศัพท์โรงแรม

        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty; // อีเมลโรงแรม

        [Url(ErrorMessage = "Invalid website format.")]
        public string Website { get; set; } = string.Empty; // เว็บไซต์ (ถ้ามี)

        // ความสัมพันธ์กับสิ่งอำนวยความสะดวก
        public ICollection<HotelAmenity> HotelAmenities { get; set; } = new List<HotelAmenity>();

        // ความสัมพันธ์กับห้องพัก
        public ICollection<Room> Rooms { get; set; } = new List<Room>();

        // ความสัมพันธ์กับรูปภาพ
        public ICollection<HotelImage> Images { get; set; } = new List<HotelImage>();

        // การจัดการสถานะ
        [Required]
        public bool IsActive { get; set; } = true; // โรงแรมเปิดให้บริการหรือไม่

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // วันที่สร้าง

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // วันที่แก้ไขล่าสุด

        // ฟังก์ชันช่วย
        public string FullAddress => $"{Address}, {City}, {Country}, {ZipCode}";

        // คะแนนโรงแรม
        [Range(0, 5, ErrorMessage = "Rating must be between 0 and 5.")]
        public double Rating { get; set; } = 0;

        // จำนวนรีวิว
        public int ReviewCount { get; set; } = 0;

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateDetails(
            string name, string description, string address, string city, string country,
            string zipCode, string phone, string email, string website)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(address) ||
                string.IsNullOrWhiteSpace(city) || string.IsNullOrWhiteSpace(country) ||
                string.IsNullOrWhiteSpace(zipCode))
            {
                throw new ArgumentException("Required fields cannot be empty.");
            }

            Name = WebUtility.HtmlEncode(name.Trim());
            Description = WebUtility.HtmlEncode(description?.Trim() ?? string.Empty);
            Address = WebUtility.HtmlEncode(address.Trim());
            City = WebUtility.HtmlEncode(city.Trim());
            Country = WebUtility.HtmlEncode(country.Trim());
            ZipCode = WebUtility.HtmlEncode(zipCode.Trim());
            Phone = phone?.Trim() ?? string.Empty;
            Email = email?.Trim() ?? string.Empty;
            Website = website?.Trim() ?? string.Empty;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateRating(double newRating, int newReviewCount)
        {
            if (newReviewCount <= 0 || newRating < 0 || newRating > 5)
                throw new ArgumentException("Invalid rating or review count.");

            Rating = newRating;
            ReviewCount = newReviewCount;
        }
    }
}
