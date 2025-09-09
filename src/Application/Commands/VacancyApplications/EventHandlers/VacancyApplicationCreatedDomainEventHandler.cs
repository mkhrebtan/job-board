using Application.Services;
using Domain.Abstraction.Interfaces;
using Domain.Contexts.ApplicationContext.Events;
using Domain.Repos.ReadModels;
using Domain.Shared.Events;

namespace Application.Commands.VacancyApplications.EventHandlers;

internal sealed class VacancyApplicationCreatedDomainEventHandler : IDomainEventHandler<VacancyApplicationCreatedDomainEvent>
{
    private readonly IVacancyApplicationsReadModelService _vacancyApplicationsReadModelService;
    private readonly IVacancyApplicationsReadModelRepository _vacancyApplicationsReadModelRepository;
    private readonly IUnitOfWork _unitOfWork;

    public VacancyApplicationCreatedDomainEventHandler(
        IVacancyApplicationsReadModelService vacancyApplicationsReadModelService,
        IVacancyApplicationsReadModelRepository vacancyApplicationsReadModelRepository,
        IUnitOfWork unitOfWork)
    {
        _vacancyApplicationsReadModelService = vacancyApplicationsReadModelService;
        _vacancyApplicationsReadModelRepository = vacancyApplicationsReadModelRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(VacancyApplicationCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var model = await _vacancyApplicationsReadModelService.GenerateReadModelAsync(domainEvent.VacancyApplicationId, cancellationToken);
        if (model is not null)
        {
            _vacancyApplicationsReadModelRepository.Add(model);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
