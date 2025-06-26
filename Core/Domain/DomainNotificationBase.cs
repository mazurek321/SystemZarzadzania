using System;

namespace Core.Domain;

public abstract class DomainNotificationBase<T> : DomainEventBase
{
    public T Data { get; }

    protected DomainNotificationBase(T data)
    {
        Data = data;
        OccurredOn = DateTime.UtcNow;
    }
}
