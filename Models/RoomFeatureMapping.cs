using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ForMiraiProject.Models  // Ensure this is your project namespace
{
    public class RoomFeatureMapping
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } // Primary Key

        [Required]
        public Guid RoomId { get; set; } // Foreign Key to Room

        [Required]
        public Room Room { get; set; } = null!; // Relationship with Room

        [Required]
        public Guid FeatureId { get; set; } // Foreign Key to Feature

        [Required]
        public Feature Feature { get; set; } = null!; // Relationship with Feature

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Date when this relation is created

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // Last date this relation was updated

        // Helper function to update details
        public void UpdateDetails(Guid roomId, Guid featureId)
        {
            if (roomId == Guid.Empty || featureId == Guid.Empty)
                throw new ArgumentException("RoomId and FeatureId cannot be empty");

            RoomId = roomId;
            FeatureId = featureId;
            UpdatedAt = DateTime.UtcNow;
        }

        // Validate if RoomId and FeatureId are valid (not empty)
        public static bool ValidateRoomFeature(Guid roomId, Guid featureId)
        {
            return roomId != Guid.Empty && featureId != Guid.Empty;
        }

        // Ensure relationship is valid (e.g., Room and Feature must exist)
        public void ValidateRelationship(Func<Guid, bool> roomExists, Func<Guid, bool> featureExists)
        {
            if (!roomExists(RoomId))
                throw new InvalidOperationException($"Room with ID {RoomId} does not exist.");
            
            if (!featureExists(FeatureId))
                throw new InvalidOperationException($"Feature with ID {FeatureId} does not exist.");
        }

        // Sanitize input data (if any external input is present)
        public void SanitizeInput()
        {
            // Assuming Room and Feature have no strings that need sanitizing,
            // this method is left empty, but can be expanded if needed.
        }

        // Deactivate the mapping
        public void Deactivate()
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
