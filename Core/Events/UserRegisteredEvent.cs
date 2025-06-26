using System;
using Core.Models.Users;
using Core.Domain;

namespace Core.Events;

public class UserRegisteredEvent : DomainEventBase
{
    public User User { get; }

    public UserRegisteredEvent(User user)
    {
        User = user;
        OccurredOn = DateTime.UtcNow;
    }
}
