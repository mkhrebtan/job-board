using Domain.Shared.Events;

namespace Domain.Abstraction;

public interface IAggregateRoot
{
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    void ClearDomainEvents();

    void RaiseDomainEvent(IDomainEvent domainEvent);
}
