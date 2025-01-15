namespace ForMiraiProject.Services
{
    using ForMiraiProject.ViewModels;
    using System.Threading.Tasks;

    public interface I_NotificationService
    {
        Task<bool> SendNotificationAsync(NotificationRequestViewModel model);
    }
}
