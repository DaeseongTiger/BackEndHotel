using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace ForMiraiProject.Models
{
    public class Room
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } // Primary Key

        [Required(ErrorMessage = "Room number is required")]
        [MaxLength(20, ErrorMessage = "Room number cannot exceed 20 characters.")]
        public string RoomNumber { get; set; } = string.Empty; // หมายเลขห้องพัก

        [Required(ErrorMessage = "Price per night is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Price per night must be a positive value.")]
        public decimal PricePerNight { get; set; } // ราคาต่อคืน

        [MaxLength(100, ErrorMessage = "Room type cannot exceed 100 characters.")]
        public string RoomType { get; set; } = string.Empty; // ประเภทห้องพัก เช่น Deluxe, Suite

        [Range(1, int.MaxValue, ErrorMessage = "Max occupancy must be at least 1.")]
        public int MaxOccupancy { get; set; } = 1; // จำนวนผู้เข้าพักสูงสุด

        [Required(ErrorMessage = "Hotel ID is required")]
        public Guid HotelId { get; set; } // Foreign Key ไปยังโรงแรม
        public Hotel? Hotel { get; set; } // ความสัมพันธ์กับ Hotel (nullable Hotel)

        // ความสัมพันธ์กับคุณสมบัติห้อง
        public ICollection<RoomFeatureMapping> RoomFeatures { get; set; } = new List<RoomFeatureMapping>();

        // ความสัมพันธ์กับการจอง
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

        // สถานะห้องพัก
        [Required]
        public bool IsAvailable { get; set; } = true; // ห้องพักพร้อมใช้งานหรือไม่

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // วันที่สร้างห้องพัก

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // วันที่แก้ไขข้อมูลล่าสุด

        // ฟังก์ชันช่วยเพื่อปรับสถานะห้อง
        public void MarkAsUnavailable()
        {
            IsAvailable = false;
            UpdatedAt = DateTime.UtcNow;
        }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
        public decimal Price { get; set; } // ราคาเพิ่มเติมจากการคำนวณ

        // อัปเดตราคาห้องโดยการเพิ่มค่าธรรมเนียม
        public void UpdatePrice(decimal additionalFees)
        {
            if (additionalFees < 0)
                throw new ArgumentException("Additional fees must not be negative.");

            Price = PricePerNight + additionalFees;
        }

        public void MarkAsAvailable()
        {
            IsAvailable = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateDetails(string roomNumber, decimal pricePerNight, string roomType, int maxOccupancy)
        {
            if (string.IsNullOrWhiteSpace(roomNumber))
                throw new ArgumentException("Room number cannot be empty or null.");

            if (pricePerNight < 0)
                throw new ArgumentException("Price per night must be a positive value.");

            if (string.IsNullOrWhiteSpace(roomType))
                throw new ArgumentException("Room type cannot be empty or null.");

            if (maxOccupancy < 1)
                throw new ArgumentException("Max occupancy must be at least 1.");

            RoomNumber = WebUtility.HtmlEncode(roomNumber);
            PricePerNight = pricePerNight;
            RoomType = WebUtility.HtmlEncode(roomType);
            MaxOccupancy = maxOccupancy;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
