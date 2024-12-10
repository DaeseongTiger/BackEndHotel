using System.ComponentModel.DataAnnotations;

namespace ForMiraiProject.DTOs
{
    /// <summary>
    /// DTO สำหรับอัปเดตข้อมูลโรงแรม
    /// </summary>
    public class HotelUpdateRequest
    {
        [Required]
        public Guid Id { get; set; } // Primary Key

        [Required(ErrorMessage = "Hotel name is required")]
        [MaxLength(100, ErrorMessage = "Hotel name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required")]
        [MaxLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required")]
        [MaxLength(50, ErrorMessage = "City cannot exceed 50 characters.")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Country is required")]
        [MaxLength(50, ErrorMessage = "Country cannot exceed 50 characters.")]
        public string Country { get; set; } = string.Empty;

        [Required(ErrorMessage = "ZipCode is required")]
        [MaxLength(10, ErrorMessage = "ZipCode cannot exceed 10 characters.")]
        public string ZipCode { get; set; } = string.Empty;

        [MaxLength(15, ErrorMessage = "Phone number cannot exceed 15 characters.")]
        public string Phone { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        [Url(ErrorMessage = "Invalid website format.")]
        public string Website { get; set; } = string.Empty;
    }
}
