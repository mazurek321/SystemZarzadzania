using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Api.Hubs;
using Core.Models.Notifications;

namespace Api.Services;
public class NotificationSender : INotificationSender
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationSender(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendNotificationToUserAsync(Guid userId, string message)
    {
        await _hubContext.Clients.User(userId.ToString())
                .SendAsync("ReceiveNotification", message);
    }
}

