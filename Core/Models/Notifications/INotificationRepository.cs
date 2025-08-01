namespace Core.Models.Notifications;

public interface INotificationRepository
{
    Task AddAsync(Notification notification);
    Task<List<Notification>> BrowseNotifications(Guid? userId, bool? unread);
    Task<Notification> GetNotification(Guid notificationId);
    Task<bool> CheckIfAlreadyExistsAsync(Guid userId, string message);
    Task UpdateAsync(Notification notif);
}