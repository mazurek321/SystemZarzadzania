using Microsoft.AspNetCore.SignalR;
using Core.Models.Notifications;

namespace Api.Hubs;

public class NotificationHub : Hub
{
    public async Task SendNotificationToUser(Notification notification)
    {
        await Clients.User(notification.UserId.ToString()).SendAsync("ReceiveNotification", notification);
    }

}