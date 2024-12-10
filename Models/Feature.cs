using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForMiraiProject.Models
{
    public class Feature
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } // Primary Key

        [Required(ErrorMessage = "Feature name is required.")]
        [MaxLength(100, ErrorMessage = "Feature name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty; // Name of the feature (e.g., Wi-Fi, Parking)

        [Required(ErrorMessage = "Description is required.")]
        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; } = string.Empty; // Description of the feature (e.g., Free Wi-Fi, 24/7 Parking)

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Creation timestamp

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // Last updated timestamp

        [Required]
        public bool IsActive { get; set; } = true; // Status of the feature (active/inactive)

        // Method to deactivate the feature
        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        // Method to update feature details with validation
        public void UpdateDetails(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Feature name cannot be empty or null.", nameof(name));

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be empty or null.", nameof(description));

            Name = Sanitize(name);
            Description = Sanitize(description);
            UpdatedAt = DateTime.UtcNow;
        }

        // Helper method to sanitize inputs (to prevent XSS attacks)
        private string Sanitize(string input)
        {
            return System.Net.WebUtility.HtmlEncode(input);
        }

        // Static method to validate the 'IsActive' status
        public static bool ValidateIsActive(bool isActive)
        {
            return isActive;
        }

        // Validation method to check feature properties before persisting
        public bool ValidateFeature()
        {
            if (string.IsNullOrWhiteSpace(Name) || Name.Length > 100)
                throw new ValidationException("Feature name is invalid or exceeds 100 characters.");
            if (!string.IsNullOrWhiteSpace(Description) && Description.Length > 500)
                throw new ValidationException("Description exceeds 500 characters.");
            return true;
        }
    }
}
