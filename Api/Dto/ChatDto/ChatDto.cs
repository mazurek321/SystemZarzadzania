using Core.Models.Chats;
using System.ComponentModel.DataAnnotations;

namespace Api.Dto.ChatDto;
public class ChatDto
{
    public Guid Id { get; init; }
    public string? Name { get; init; }
    public List<Guid> Participants { get; init; } = new List<Guid>();
    public List<Guid> Messages { get; init; } = new List<Guid>();
}