using Core.Models.Users;

namespace Core.Models.Notifications;

public class Notification
{
    public Notification() { }
    public Notification(
        Guid id, NotificationType type, Guid userId, string message, DateTime createdAt, bool isRead, DateTime? readAt, DateTime receivedAt
    )
    {
        Id = id;
        Type = type;
        UserId = userId;
        Message = message;
        CreatedAt = createdAt;
        IsRead = isRead;
        ReadAt = readAt;
        ReceivedAt = receivedAt;
    }
    public Guid Id { get; private set; }
    public NotificationType Type { get; private set; }
    public Guid UserId { get; private set; }
    public string Message { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsRead { get; private set; }
    public DateTime? ReadAt{ get;  private set; }
    public DateTime ReceivedAt { get; private set; }

    public static Notification NewNotification(NotificationType type, Guid userId, string message)
    {
        return new Notification(Guid.NewGuid(), type, userId, message, DateTime.UtcNow, false, null, DateTime.UtcNow);
    }

    public void MarkAsRead()
    {
        IsRead = true;
        ReadAt = DateTime.UtcNow;
    }
}

public enum NotificationType
{
    Alert,
    Warning,
    Normal
}