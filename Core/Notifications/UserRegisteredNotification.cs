using Newtonsoft.Json;
using Core.Events;
using Core.Models;
using Core.Domain;

namespace Core.Notifications;

public class UserRegisteredNotification : DomainNotificationBase<UserRegisteredEvent>
{
    public Guid UserId { get; }

    public UserRegisteredNotification(UserRegisteredEvent domainEvent) : base(domainEvent)
    {
        UserId = domainEvent.User.Id;
    }

    [JsonConstructor]
    public UserRegisteredNotification(Guid userId) : base(null)
    {
        UserId = userId;
    }
}
