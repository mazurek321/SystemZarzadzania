using MediatR;
namespace Core.Domain;

public abstract class DomainEventBase : INotification
{
    public DateTime OccurredOn { get; protected set; }
}