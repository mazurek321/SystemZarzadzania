using System;
using Core.Models.Users;
using Core.Domain;
using Core.Entities;
using MediatR;

namespace Core.Events;

public class UserRegisteredEvent : DomainEventBase, INotification
{
    public UserWithEvents User { get; }

    public UserRegisteredEvent(UserWithEvents user)
    {
        User = user;
        OccurredOn = DateTime.UtcNow;
    }
}
