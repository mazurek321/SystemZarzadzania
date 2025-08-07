namespace Core.Models.Messages;

public class Message
{
    public Message(Guid id, string messageContent, Guid senderUserId, Guid receiverUserId, DateTime sentAt)
    {
        Id = id;
        MessageContent = messageContent;
        SenderUserId = senderUserId;
        ReceiverUserId = receiverUserId;
        SentAt = sentAt;
    }

    public Guid Id { get; private set; }
    public string MessageContent { get; private set; }
    public Guid SenderUserId { get; private set; }
    public Guid ReceiverUserId { get; private set; }
    public DateTime SentAt { get; private set; }
    public bool? IsRead { get; private set; }
    public DateTime? ReadAt { get; private set; }

    public static Message NewMessage(string messageContent, Guid senderUserId, Guid receiverUserId)
    {
        return new Message(Guid.NewGuid(), messageContent, senderUserId, receiverUserId, DateTime.UtcNow);
    }

    public void UpdateReadMessage()
    {
        IsRead = true;
        ReadAt = DateTime.UtcNow;
    }
}