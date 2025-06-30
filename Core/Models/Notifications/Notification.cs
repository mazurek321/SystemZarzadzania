using Core.Models.Users;

namespace Core.Models.Notifications;

public class Notification
{
    public Notification() { }
    public Notification(
        Guid id, NotificationType type, User forUser, string message, DateTime createdAt, bool isRead
    )
    {
        Id = id;
        Type = type;
        ForUser = forUser;
        Message = message;
        CreatedAt = createdAt;
        IsRead = isRead;
    }
    public Guid Id { get; private set; }
    public NotificationType Type { get; private set; }
    public User ForUser { get; private set; }
    public string Message { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsRead { get; private set; }

    public static Notification NewNotification(NotificationType type, User forUser, string message)
    {
        return new Notification(Guid.NewGuid(), type, forUser, message, DateTime.UtcNow, false);
    }

    public void MarkAsRead()
    {
        IsRead = true;
    }
}

public enum NotificationType
{
    Alert,
    Reminder,
    Normal
}