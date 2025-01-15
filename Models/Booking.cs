using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace ForMiraiProject.Models
{
    public class Booking
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } // Primary Key

        [Required]
        public Guid UserId { get; set; } // Foreign Key to User

        [Required]
        public User User { get; set; } = default!; // Relationship with User (Non-nullable with default!)

        [Required]
        public Guid RoomId { get; set; } // Foreign Key to Room

        [Required]
        public Room Room { get; set; } = default!; // Relationship with Room (Non-nullable with default!)

        public Guid? CouponId { get; set; } // Optional Foreign Key to Coupon

        public Coupon? Coupon { get; set; } // Navigation Property for Coupon

        [Required]
        public DateTime CheckInDate { get; set; } // Check-in date

        [Required]
        public DateTime CheckOutDate { get; set; } // Check-out date

        public bool IsCancelled { get; set; } = false; // Booking cancellation status

        [MaxLength(500)]
        public string SpecialRequests { get; set; } = string.Empty; // Special requests (Optional)

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Total amount must be greater than zero.")]
        public decimal TotalAmount { get; set; } // Total booking amount

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Booking creation date

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; 
        
        public string? UserName { get; set; } // Last updated date

        public DateTime StartDate { get; set; } // วันที่เริ่มต้นของการจอง
        public DateTime EndDate { get; set; }

        public string BookingStatus { get; set; } = "Pending";
        public DateTime ExpiryDate { get; set; }

        public  string? CustomerName { get; set; }
        public DateTime BookingDate { get; set; }




        

        // Default constructor for EF Core
        public Booking()
        {
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        // Cancel booking method
        public void CancelBooking(Action<string>? logAction = null)
        {
            IsCancelled = true;
            UpdatedAt = DateTime.UtcNow;

            // Log cancellation action
            logAction?.Invoke($"Booking {Id} cancelled at {UpdatedAt}.");
        }

        // Update booking dates
        public void UpdateDates(DateTime checkInDate, DateTime checkOutDate)
        {
            if (checkInDate >= checkOutDate)
                throw new ArgumentException("Check-Out date must be later than Check-In date.", nameof(checkOutDate));

            CheckInDate = checkInDate;
            CheckOutDate = checkOutDate;
            UpdatedAt = DateTime.UtcNow;
        }

        // Update special requests, includes XSS prevention
        public void UpdateSpecialRequests(string specialRequests)
        {
            if (string.IsNullOrWhiteSpace(specialRequests))
                throw new ArgumentException("Special request cannot be empty.", nameof(specialRequests));

            SpecialRequests = WebUtility.HtmlEncode(specialRequests); // Prevent XSS by HTML Encoding
            UpdatedAt = DateTime.UtcNow;
        }

        // Security: Validate user ownership before update
        public void ValidateOwnership(Guid userId)
        {
            if (userId != UserId)
                throw new UnauthorizedAccessException("You are not authorized to modify this booking.");
        }

        // Security: Validate booking dates
        public void ValidateBookingDates()
        {
            if (CheckInDate >= CheckOutDate)
                throw new InvalidOperationException("Check-In date must be earlier than Check-Out date.");
        }

        // Apply coupon to booking
        public void ApplyCoupon(Coupon coupon)
        {
            if (coupon != null && coupon.IsExpired && coupon.IsActive)
            {
                CouponId = coupon.Id;
                Coupon = coupon;

                var discountAmount = TotalAmount * (coupon.DiscountPercentage / 100);
                TotalAmount -= Math.Min(discountAmount, coupon.MaxDiscountAmount);
                UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                throw new InvalidOperationException("The provided coupon is invalid or expired.");
            }
        }
    }
}
