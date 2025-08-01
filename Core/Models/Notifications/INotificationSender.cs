using System;
using System.Threading.Tasks;

namespace Core.Models.Notifications;

public interface INotificationSender
{
    Task SendNotificationToUserAsync(Guid userId, NotificationType type, string message);
    
}
