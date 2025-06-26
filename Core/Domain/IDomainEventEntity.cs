using MediatR;
namespace Core.Domain;

public interface IDomainEventEntity : INotification
{
    List<DomainEventBase> DomainEvents { get; }
    void ClearDomainEvents();
}

