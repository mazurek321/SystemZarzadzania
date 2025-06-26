namespace Core.Domain;

public interface IDomainEventEntity
{
    List<DomainEventBase> DomainEvents { get; }
    void ClearDomainEvents();
}

