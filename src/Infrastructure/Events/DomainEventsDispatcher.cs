using System.Reflection;
using Application.Abstraction.Events;
using Domain.Shared.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Events;

internal sealed class DomainEventsDispatcher : IDomainEventsDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public DomainEventsDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        using IServiceScope scope = _serviceProvider.CreateScope();
        foreach (var domainEvent in domainEvents)
        {
            var handlers = scope.ServiceProvider.GetServices(typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType()));

            foreach (var handler in handlers)
            {
                if (handler is null)
                {
                    continue;
                }

                MethodInfo? methodInfo = handler.GetType().GetMethod("Handle");
                await (Task)methodInfo?.Invoke(handler, new object[] { domainEvent, cancellationToken })!;
            }
        }
    }
}