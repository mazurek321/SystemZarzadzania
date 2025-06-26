using System.Collections.Generic;
using Core.Events;
using Core.Domain;
using Core.Models.Users;

public class UserWithEvents : User, IDomainEventEntity
{
    public UserWithEvents(Guid id, string name, string lastname, string password, string email, UserRole role, DateTime createdAt, DateTime updatedAt)
        : base(id, name, lastname, password, email, role, createdAt, updatedAt)
    {
        DomainEvents = new List<DomainEventBase>();
    }

    public List<DomainEventBase> DomainEvents { get; private set; }

    public void AddDomainEvent(DomainEventBase domainEvent)
    {
        DomainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        DomainEvents.Clear();
    }
}
