using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Api.Hubs;
using Core.Models.Notifications;

namespace Api.Services;
public class NotificationSender : INotificationSender
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly INotificationRepository _notificationRepository;

    public NotificationSender(
        IHubContext<NotificationHub> hubContext,
        INotificationRepository notificationRepository
    )
    {
        _hubContext = hubContext;
        _notificationRepository = notificationRepository;
    }

    public async Task SendNotificationToUserAsync(Guid userId, NotificationType type, string message)
    {
        var notification = Notification.NewNotification(
            type,
            userId,
            message
        );

        await _notificationRepository.AddAsync(notification);

        await _hubContext.Clients.User(notification.UserId.ToString())
                .SendAsync("ReceiveNotification", notification.Id, notification.Type.ToString(), notification.Message, notification.IsRead, notification.ReceivedAt);
    }
}

