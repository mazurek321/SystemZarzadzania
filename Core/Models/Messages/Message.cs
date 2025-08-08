namespace Core.Models.Messages;

public class Message
{
    public Message(Guid id, string messageContent, Guid senderUserId, DateTime sentAt)
    {
        Id = id;
        MessageContent = messageContent;
        SenderUserId = senderUserId;
        SentAt = sentAt;
    }

    public Guid Id { get; private set; }
    public string MessageContent { get; private set; }
    public Guid SenderUserId { get; private set; }
    public DateTime SentAt { get; private set; }
    public List<MessageStatus> Statuses { get; private set; } = new List<MessageStatus>();

    public static Message NewMessage(string messageContent, Guid senderUserId)
    {
        return new Message(Guid.NewGuid(), messageContent, senderUserId, DateTime.UtcNow);
    }

    public void MarkAsRead(Guid userId)
    {
        var status = Statuses.Find(x => x.UserId == userId);
        if (!status.IsRead)
            status.MarkAsRead();
    }

    public bool IsReadBy(Guid userId)
    {
        var status = Statuses.Find(x => x.UserId == userId);
        return status.IsRead;
    }
}