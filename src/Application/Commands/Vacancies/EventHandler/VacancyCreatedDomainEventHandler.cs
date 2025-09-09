using Application.Services;
using Domain.Abstraction.Interfaces;
using Domain.Contexts.JobPostingContext.Events;
using Domain.Repos.ReadModels;
using Domain.Shared.Events;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Vacancies.EventHandler;

internal sealed class VacancyCreatedDomainEventHandler : IDomainEventHandler<VacancyCreatedDomainEvent>
{
    private readonly ICompanyVacanciesReadModelService _companyVacanciesReadModelService;
    private readonly ICompanyVacanciesReadModelRepository _companyVacanciesReadModelRepository;
    private readonly ILogger<VacancyCreatedDomainEventHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public VacancyCreatedDomainEventHandler(
        ICompanyVacanciesReadModelService companyVacanciesReadModelService,
        ICompanyVacanciesReadModelRepository companyVacanciesReadModelRepository,
        ILogger<VacancyCreatedDomainEventHandler> logger,
        IUnitOfWork unitOfWork)
    {
        _companyVacanciesReadModelService = companyVacanciesReadModelService;
        _companyVacanciesReadModelRepository = companyVacanciesReadModelRepository;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(VacancyCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var readModel = await _companyVacanciesReadModelService.GenerateReadModelAsync(domainEvent.VacancyId, cancellationToken);
        if (readModel is not null)
        {
            _companyVacanciesReadModelRepository.Add(readModel);
            await _unitOfWork.SaveChangesAsync();
        }
        else
        {
            _logger.LogError("Failed to generate CompanyVacanciesReadModel for VacancyId: {VacancyId}", domainEvent.VacancyId);
        }
    }
}