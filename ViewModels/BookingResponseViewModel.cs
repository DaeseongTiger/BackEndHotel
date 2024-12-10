using System;
using System.ComponentModel.DataAnnotations;

namespace ForMiraiProject.ViewModels
{
    public class BookingResponseViewModel
    {
        /// <summary>
        /// ID ของการจอง
        /// </summary>
        [Required]
        public Guid BookingId { get; set; }

        public Guid UserId { get; set; }

        public string BookingStatus { get; set; } = string.Empty;

        public BookingResponseViewModel() { }

        /// <summary>
        /// ID ของห้องที่ทำการจอง
        /// </summary>
        [Required]
        public Guid RoomId { get; set; }

        /// <summary>
        /// ชื่อของผู้ที่ทำการจอง
        /// </summary>
        [Required(ErrorMessage = "Full name is required.")]
        [MaxLength(100, ErrorMessage = "Full name cannot exceed 100 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Full name can only contain letters and spaces.")]
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// อีเมลของผู้ที่ทำการจอง
        /// </summary>
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [MaxLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// หมายเลขโทรศัพท์ของผู้ที่ทำการจอง
        /// </summary>
        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "Invalid phone number format.")]
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// วันที่เริ่มต้นการจอง
        /// </summary>
        [Required(ErrorMessage = "Start date is required.")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// วันที่สิ้นสุดการจอง
        /// </summary>
        [Required(ErrorMessage = "End date is required.")]
        [DataType(DataType.Date)]
        [CustomDateRange(ErrorMessage = "End date must be later than start date.")]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// สถานะการจอง (เช่น "Confirmed", "Pending", "Cancelled")
        /// </summary>
        [Required(ErrorMessage = "Booking status is required.")]
        [MaxLength(50, ErrorMessage = "Booking status cannot exceed 50 characters.")]
        public string Status { get; set; } = "Pending";

        /// <summary>
        /// คำขอพิเศษ (ถ้ามี)
        /// </summary>
        [MaxLength(500, ErrorMessage = "Special requests cannot exceed 500 characters.")]
        public string? SpecialRequests { get; set; }

        public BookingResponseViewModel(string bookingStatus)
    {
        BookingStatus = bookingStatus ?? throw new ArgumentNullException(nameof(bookingStatus));
    }
    

        /// <summary>
        /// ข้อความยืนยันการจอง
        /// </summary>
        public string ConfirmationMessage => $"Booking for {FullName} from {StartDate.ToShortDateString()} to {EndDate.ToShortDateString()} is {Status}.";

} }
