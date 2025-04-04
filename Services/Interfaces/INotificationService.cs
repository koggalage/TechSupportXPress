namespace TechSupportXPress.Services.Interfaces
{
    public interface INotificationService
    {
        Task NotifyAdminsAsync(string message);
        Task NotifyUserAsync(string userId, string message);

    }
}
