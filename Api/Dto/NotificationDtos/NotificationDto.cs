namespace Api.Dto.NotificationDto;

public class NotificationDto
{
    public Guid Id {get; init;}
    public string Type {get; init;}
    public Guid UserId {get; init;}
    public string Message {get; init;}
    public bool IsRead {get; init;}
    public DateTime? ReadAt { get;  init;}
    public DateTime ReceivedAt { get; init; }
}