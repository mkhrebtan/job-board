using API.Authentication;
using Application.Services;
using Domain.Abstraction.Interfaces;
using Domain.Contexts.IdentityContext.IDs;
using Domain.Contexts.JobPostingContext.Events;
using Domain.Repos.ReadModels;
using Domain.Shared.Events;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Vacancies.EventHandler;

internal sealed class VacancyRegisteredDomainEventHandler : IDomainEventHandler<VacancyRegisteredDomainEvent>
{
    private readonly IRegisteredVacanciesReadModelService _registeredVacanciesReadModelService;
    private readonly IRegisteredVacanciesReadModelRepository _registeredVacanciesReadModelRepository;
    private readonly ICompanyVacanciesReadModelRepository _companyVacanciesReadModelRepository;
    private readonly ILogger<VacancyRegisteredDomainEvent> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public VacancyRegisteredDomainEventHandler(
        IRegisteredVacanciesReadModelService registeredVacanciesReadModelService,
        IRegisteredVacanciesReadModelRepository registeredVacanciesReadModelRepository,
        ICompanyVacanciesReadModelRepository companyVacanciesReadModelRepository,
        ILogger<VacancyRegisteredDomainEvent> logger,
        IUnitOfWork unitOfWork,
        IUserContext userContext)
    {
        _registeredVacanciesReadModelService = registeredVacanciesReadModelService;
        _registeredVacanciesReadModelRepository = registeredVacanciesReadModelRepository;
        _companyVacanciesReadModelRepository = companyVacanciesReadModelRepository;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task Handle(VacancyRegisteredDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var readModel = await _registeredVacanciesReadModelService.GenerateReadModelAsync(domainEvent.VacancyId, new UserId(_userContext.UserId));
        if (readModel is not null)
        {
            _registeredVacanciesReadModelRepository.Add(readModel);
            await _companyVacanciesReadModelRepository.Update(domainEvent.VacancyId.Value);

            await _unitOfWork.SaveChangesAsync();
        }
        else
        {
            _logger.LogError("Failed to generate RegisteredVacanciesReadModel for VacancyId: {VacancyId}", domainEvent.VacancyId);
        }
    }
}
