using Core.Models.Chats;
using System.ComponentModel.DataAnnotations;
public class CreateChatDto
{
    public string? Name { get; init; }
    public List<Guid> Participants { get; init; } = new List<Guid>();
}