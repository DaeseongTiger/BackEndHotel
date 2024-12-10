using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ForMiraiProject.ViewModels
{
    public class BookingRequestViewModel : IValidatableObject
    {
        [Required(ErrorMessage = "User ID is required.")]
        public Guid UserId { get; set; }

        [Required(ErrorMessage = "Room ID is required.")]
        public Guid RoomId { get; set; }

        [Required(ErrorMessage = "Start date is required.")]
        [CustomDateRange(ErrorMessage = "Start date must be a valid date and not in the past.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required.")]
        [CustomDateRange(ErrorMessage = "End date must be a valid date and later than the start date.")]
        public DateTime EndDate { get; set; }

        [StringLength(500, ErrorMessage = "Special requests cannot exceed 500 characters.")]
        public string? SpecialRequests { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndDate <= StartDate)
            {
                yield return new ValidationResult(
                    "End date must be later than the start date.",
                    new[] { nameof(EndDate) });
            }
        }
    } }


