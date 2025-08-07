using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Context;
public class SignalRUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        return connection.User?.FindFirst("userid")?.Value;
    }
}