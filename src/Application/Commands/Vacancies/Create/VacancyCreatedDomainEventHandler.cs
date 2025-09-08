using Domain.Contexts.JobPostingContext.Events;
using Domain.Shared.Events;

namespace Application.Commands.Vacancies.Create;

internal sealed class VacancyCreatedDomainEventHandler : IDomainEventHandler<VacancyCreatedDomainEvent>
{
    public Task Handle(VacancyCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}

internal sealed class VacancyCreatedDomainEventHandler2 : IDomainEventHandler<VacancyCreatedDomainEvent>
{
    public Task Handle(VacancyCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
