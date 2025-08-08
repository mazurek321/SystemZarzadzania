namespace Core.Models.Chats;

public class Chat
{
    public Chat(Guid id, string? name, List<Guid> participants)
    {
        Id = id;
        Name = name;
        Participants = participants;
    }

    public Guid Id { get; private set; }
    public string? Name { get; private set; }
    public List<Guid> Participants { get; private set; } = new List<Guid>();
    public List<Guid> Messages { get; private set; } = new List<Guid>();

    public static Chat NewChat(string? name, List<Guid> participants)
    {
        return new Chat(Guid.NewGuid(), name, participants);
    }

    public void AddParticipant(Guid userId)
    {
        if (!Participants.Contains(userId))
            Participants.Add(userId);
    }

    public void RemoveParticipant(Guid userId)
    {
        if (Participants.Contains(userId))
            Participants.Remove(userId);
    }

    public void AddMessage(Guid messageId)
    {
        if (!Messages.Contains(messageId))
            Messages.Add(messageId);
    }

    public void DeleteMessage(Guid messageId)
    {
        if (Messages.Contains(messageId))
            Messages.Remove(messageId);
    }

    public void Rename(string newName)
    {
        Name = newName;
    }

}