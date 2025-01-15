using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace ForMiraiProject.Models
{
    public class Coupon
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } // Primary Key

        [Required]
        [MaxLength(20, ErrorMessage = "Code cannot exceed 20 characters.")]
        public string Code { get; set; } = string.Empty;
        
         // Coupon Code

        [Required]
        [Range(0, 100, ErrorMessage = "Discount percentage must be between 0 and 100.")]
        public decimal DiscountPercentage { get; set; } // Discount percentage

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Maximum discount amount must be greater than or equal to 0.")]
        public decimal MaxDiscountAmount { get; set; } // Maximum discount amount

        [Required]
        [DataType(DataType.DateTime, ErrorMessage = "Invalid date format.")]
        public DateTime ExpiryDate { get; set; } // Expiry date

        [Required]
        public bool IsActive { get; set; } = true; // Active status

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; } = string.Empty; // Description

        public ICollection<User> RestrictedUsers { get; set; } = new List<User>(); // Restricted users

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Creation timestamp

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime ExpirationDate { get; set; } // Update timestamp

        public bool IsUsed { get; set; } // หรืออาจจะเป็นฟังก์ชันที่คอยตรวจสอบ
        public decimal DiscountAmount { get; set; }

        public List<Guid> RestrictedUserIds { get; set; }

        public bool IsExpired => DateTime.Now > ExpiryDate;

        // Method to check if coupon is expired or inactive
        public bool IsExpiredOrInactive() 
        {
            return !IsActive || DateTime.UtcNow > ExpiryDate;
        }

        // Method to deactivate coupon
        public void Deactivate()
        {
            if (!IsActive) return; // Avoid unnecessary updates

            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        // Validate and update coupon details
        public void UpdateDetails(string code, decimal discountPercentage, decimal maxDiscountAmount, DateTime expiryDate, string description)
        {
            if (Code == code && DiscountPercentage == discountPercentage &&
                MaxDiscountAmount == maxDiscountAmount && ExpiryDate == expiryDate && 
                Description == description)
            {
                // No changes detected
                return;
            }

            ValidateCouponInput(code, discountPercentage, maxDiscountAmount, expiryDate);

            Code = WebUtility.HtmlEncode(code); // Prevent XSS
            DiscountPercentage = discountPercentage;
            MaxDiscountAmount = maxDiscountAmount;
            ExpiryDate = expiryDate;
            Description = WebUtility.HtmlEncode(description); // Prevent XSS
            UpdatedAt = DateTime.UtcNow;
        }

        // Helper function to validate coupon inputs
        private void ValidateCouponInput(string code, decimal discountPercentage, decimal maxDiscountAmount, DateTime expiryDate)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Code cannot be empty.", nameof(code));

            if (discountPercentage < 0 || discountPercentage > 100)
                throw new ArgumentException("Discount percentage must be between 0 and 100.", nameof(discountPercentage));

            if (maxDiscountAmount < 0)
                throw new ArgumentException("Maximum discount amount must be greater than or equal to 0.", nameof(maxDiscountAmount));

            if (expiryDate <= DateTime.UtcNow)
                throw new ArgumentException("Expiry date must be in the future.", nameof(expiryDate));
        }

        // Method to check if discount is valid
        public bool ValidateDiscountRange()
        {
            return DiscountPercentage >= 0 && DiscountPercentage <= 100 && MaxDiscountAmount >= 0;
        }

        // Default Constructor for EF Core
    }

    // Example User class for RestrictedUsers relationsh
}
