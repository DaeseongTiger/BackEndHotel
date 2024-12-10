using System;

namespace ForMiraiProject.ViewModels
{
    public class RegisterResponseViewModel
    {
        public Guid UserId { get; set; }
        public string Message { get; set; } = "Registration successful.";

        // Default Constructor
        public RegisterResponseViewModel() { }

        // Parameterized Constructor
        public RegisterResponseViewModel(Guid userId, string message = "Registration successful.")
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty.", nameof(userId));

            UserId = userId;
            Message = message;
        }
    }
}
