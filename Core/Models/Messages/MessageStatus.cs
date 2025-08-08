namespace Core.Models.Messages;

public class MessageStatus
{
    public MessageStatus(Guid messageId, Guid userId, bool isRead)
    {
        MessageId = messageId;
        UserId = userId;
        IsRead = isRead;
        ReadAt = null;
    }

    public Guid MessageId { get; set;}
    public Guid UserId { get; private set; }
    public bool IsRead { get; private set; }
    public DateTime? ReadAt { get; private set; }

    public void MarkAsRead()
    {
        IsRead = true;
        ReadAt = DateTime.UtcNow;
    }
    
}