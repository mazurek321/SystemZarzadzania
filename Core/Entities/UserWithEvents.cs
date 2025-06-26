using System.Collections.Generic;
using Core.Events;
using Core.Domain;
using Core.Models.Users;
using MediatR;
using Newtonsoft.Json;


namespace Core.Entities;
public class UserWithEvents : User, IDomainEventEntity, INotification
{
    public UserWithEvents() 
    {
        DomainEvents = new List<DomainEventBase>();
    }

    [JsonConstructor]
    public UserWithEvents(
        Guid id,
        string name,
        string lastname,
        string passwordHash,
        string email,
        string phone,
        bool isActive,
        DateTime lastActive,
        User.UserRole role,
        DateTime createdAt,
        DateTime updatedAt)
        : base(id, name, lastname, passwordHash, email, phone, isActive, lastActive, role, createdAt, updatedAt)
    {
        DomainEvents = new List<DomainEventBase>();
    }

    public UserWithEvents(User user)
        : base(user.Id, user.Name, user.Lastname, user.PasswordHash, user.Email, user.Phone, user.IsActive, user.LastActive, user.Role, user.CreatedAt, user.UpdatedAt)
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
