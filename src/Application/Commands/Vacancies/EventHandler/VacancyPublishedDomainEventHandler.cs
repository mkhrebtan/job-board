using Application.Services;
using Domain.Abstraction.Interfaces;
using Domain.Contexts.JobPostingContext.Events;
using Domain.Repos.ReadModels;
using Domain.Shared.Events;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Vacancies.EventHandler;

internal sealed class VacancyPublishedDomainEventHandler : IDomainEventHandler<VacancyPublishedDomainEvent>
{
    private readonly IRegisteredVacanciesReadModelRepository _registeredVacanciesReadModelRepository;
    private readonly ICompanyVacanciesReadModelRepository _companyVacanciesReadModelRepository;
    private readonly IVacancyListingReadModelService _vacancyListingReadModelService;
    private readonly IVacancyListingReadModelRepository _vacancyListingReadModelRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<VacancyPublishedDomainEventHandler> _logger;

    public VacancyPublishedDomainEventHandler(
        IRegisteredVacanciesReadModelRepository registeredVacanciesReadModelRepository,
        ICompanyVacanciesReadModelRepository companyVacanciesReadModelRepository,
        IVacancyListingReadModelService vacancyListingReadModelService,
        IVacancyListingReadModelRepository vacancyListingReadModelRepository,
        IUnitOfWork unitOfWork,
        ILogger<VacancyPublishedDomainEventHandler> logger)
    {
        _registeredVacanciesReadModelRepository = registeredVacanciesReadModelRepository;
        _companyVacanciesReadModelRepository = companyVacanciesReadModelRepository;
        _vacancyListingReadModelService = vacancyListingReadModelService;
        _vacancyListingReadModelRepository = vacancyListingReadModelRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(VacancyPublishedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var readModel = await _vacancyListingReadModelService.GenerateReadModelAsync(domainEvent.VacancyId);
        if (readModel is not null)
        {
            _vacancyListingReadModelRepository.Add(readModel);
            await _registeredVacanciesReadModelRepository.Remove(domainEvent.VacancyId.Value);
            await _companyVacanciesReadModelRepository.Update(domainEvent.VacancyId.Value);

            await _unitOfWork.SaveChangesAsync();
        }
        else
        {
            _logger.LogError("Failed to generate VacancyListingReadModel for VacancyId: {VacancyId}", domainEvent.VacancyId);
        }
    }
}
