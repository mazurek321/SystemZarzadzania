namespace Core.Domain;

public abstract class DomainEventBase
{
    public DateTime OccurredOn { get; protected set; }
}