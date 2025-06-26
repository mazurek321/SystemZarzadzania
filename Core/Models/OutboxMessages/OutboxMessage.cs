using System;

namespace Core.Models.OutboxMessages;

public class OutboxMessage
{
    public Guid Id { get; private set; }
    public DateTime OccurredOn { get; private set; }
    public string Type { get; private set; }
    public string Data { get; private set; }
    public DateTime? ProcessedDate { get; private set; }

    private OutboxMessage() { }

    public OutboxMessage(DateTime occurredOn, string type, string data)
    {
        Id = Guid.NewGuid();
        OccurredOn = occurredOn;
        Type = type;
        Data = data;
    }

    public void MarkProcessed()
    {
        ProcessedDate = DateTime.UtcNow;
    }
}
