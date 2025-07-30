using Microsoft.AspNetCore.SignalR;
using Core.Models.Notifications;

namespace Api.Hubs;

public class NotificationHub : Hub
{
    public async Task SendNotificationToUser(string userId, string type, string message)
    {
        await Clients.User(userId).SendAsync("ReceiveNotification", type, message);
    }

}