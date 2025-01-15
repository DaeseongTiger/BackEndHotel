namespace ForMiraiProject.Services
{
    using ForMiraiProject.Models;
    using ForMiraiProject.ViewModels;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Threading.Tasks;

    public class NotificationService : I_NotificationService
    {
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(ILogger<NotificationService> logger)
        {
            _logger = logger;
        }

        public async Task<bool> SendNotificationAsync(NotificationRequestViewModel model)
        {
            // Validate the input model
            if (model == null)
            {
                _logger.LogError("Notification model is null.");
                return false;
            }

            try
            {
                // Create Notification entity
                var notification = new Notification
                {
                    NotificationId = Guid.NewGuid(),
                    UserId = model.UserId,
                    Message = model.Message,
                    NotificationType = model.NotificationType,
                    SentAt = DateTime.UtcNow,
                    IsDelivered = false // Initially set to false
                };

                // Simulate sending notification (e.g., via Email, SMS, or Push)
                bool isSent = await SimulateSendingNotification(notification);

                // Update the delivery status
                notification.IsDelivered = isSent;

                // Log the result
                if (isSent)
                {
                    _logger.LogInformation($"Notification sent to user {model.UserId} with message: {model.Message}");
                    return true;
                }
                else
                {
                    _logger.LogWarning($"Failed to send notification to user {model.UserId}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending notification: {ex.Message}");
                return false;
            }
        }

        // Simulate the actual sending of the notification
        private Task<bool> SimulateSendingNotification(Notification notification)
        {
            // Simulate the sending logic here (e.g., send Email, SMS, or Push Notification)
            // Return true if sent successfully, false otherwise
            return Task.FromResult(true); // Simulating a successful send
        }
    }
}
