using System;
using MediatR;

namespace Core.Domain;

public abstract class DomainNotificationBase<T> : DomainEventBase, INotification
{
    public T Data { get; }

    protected DomainNotificationBase(T data)
    {
        Data = data;
        OccurredOn = DateTime.UtcNow;
    }
}
