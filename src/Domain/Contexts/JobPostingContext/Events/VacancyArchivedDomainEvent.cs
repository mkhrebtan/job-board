using Domain.Contexts.JobPostingContext.IDs;
using Domain.Shared.Events;

namespace Domain.Contexts.JobPostingContext.Events;

public sealed record VacancyArchivedDomainEvent(VacancyId VacancyId) : IDomainEvent;
