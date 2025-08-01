using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Context;
public class SignalRUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        var context = connection.GetHttpContext();
        var userId = context?.Request.Query["userid"].FirstOrDefault();

        return Guid.TryParse(userId, out var id) ? id.ToString() : null;
    }
}