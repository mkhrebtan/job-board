using Domain.Shared.Events;

namespace Application.Abstraction.Events;

public interface IDomainEventsDispatcher
{
    Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
}

public interface IDomainEventsHandlersProvider
{
    IEnumerable<IDomainEventHandler<T>> GetHandlers<T>(T domainEvent)
        where T : IDomainEvent;
}