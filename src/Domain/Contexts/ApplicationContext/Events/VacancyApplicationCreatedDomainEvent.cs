using Domain.Contexts.ApplicationContext.IDs;
using Domain.Shared.Events;

namespace Domain.Contexts.ApplicationContext.Events;

public record VacancyApplicationCreatedDomainEvent(VacancyApplicationId VacancyApplicationId) : IDomainEvent;
