using System;
using System.ComponentModel.DataAnnotations;

namespace ForMiraiProject.DTOs
{
    // DTO สำหรับการอัปเดตข้อมูลห้อง
    public class UpdateRoomRequest
    {
        [Required(ErrorMessage = "Room number is required.")]
        [MaxLength(20, ErrorMessage = "Room number cannot be longer than 20 characters.")]
        public string RoomNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price per night is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price per night must be greater than zero.")]
        public decimal PricePerNight { get; set; }

        [Required(ErrorMessage = "Room type is required.")]
        [MaxLength(100, ErrorMessage = "Room type cannot be longer than 100 characters.")]
        public string RoomType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Max occupancy is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Max occupancy must be greater than zero.")]
        public int MaxOccupancy { get; set; }

        // Constructor ที่รองรับกรณีที่ค่า null ถูกส่ง
        public UpdateRoomRequest(string roomNumber, string roomType, decimal pricePerNight, int maxOccupancy)
        {
            RoomNumber = roomNumber ?? throw new ArgumentNullException(nameof(roomNumber));
            RoomType = roomType ?? throw new ArgumentNullException(nameof(roomType));
            PricePerNight = pricePerNight;
            MaxOccupancy = maxOccupancy;
        }
    }
}
