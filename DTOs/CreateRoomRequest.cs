using System;
using System.ComponentModel.DataAnnotations;

namespace ForMiraiProject.DTOs
{
    // DTO สำหรับการสร้างห้องใหม่
    public class CreateRoomRequest
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

        [Required(ErrorMessage = "Hotel ID is required.")]
        public Guid HotelId { get; set; }

        // Constructor ที่รองรับกรณีที่ค่า null ถูกส่ง
        public CreateRoomRequest(string roomNumber, string roomType, decimal pricePerNight, int maxOccupancy, Guid hotelId)
        {
            RoomNumber = roomNumber ?? throw new ArgumentNullException(nameof(roomNumber));
            RoomType = roomType ?? throw new ArgumentNullException(nameof(roomType));
            PricePerNight = pricePerNight;
            MaxOccupancy = maxOccupancy;
            HotelId = hotelId;
        }
    }
}
