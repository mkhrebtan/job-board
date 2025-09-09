using Domain.Abstraction.Interfaces;
using Domain.Contexts.JobPostingContext.Events;
using Domain.Repos.ReadModels;
using Domain.Shared.Events;

namespace Application.Commands.Vacancies.EventHandler;

internal sealed class VacancyDeletedDomainEventHandler : IDomainEventHandler<VacancyDeletedDomainEvent>
{
    private readonly ICompanyVacanciesReadModelRepository _companyVacanciesReadModelRepository;
    private readonly IVacancyListingReadModelRepository _vacancyListingReadModelRepository;
    private readonly IVacancyApplicationsReadModelRepository _vacancyApplicationsReadModelRepository;
    private readonly IUnitOfWork _unitOfWork;

    public VacancyDeletedDomainEventHandler(
        ICompanyVacanciesReadModelRepository companyVacanciesReadModelRepository,
        IVacancyListingReadModelRepository vacancyListingReadModelRepository,
        IVacancyApplicationsReadModelRepository vacancyApplicationsReadModelRepository,
        IUnitOfWork unitOfWork)
    {
        _companyVacanciesReadModelRepository = companyVacanciesReadModelRepository;
        _vacancyListingReadModelRepository = vacancyListingReadModelRepository;
        _vacancyApplicationsReadModelRepository = vacancyApplicationsReadModelRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(VacancyDeletedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await _companyVacanciesReadModelRepository.Remove(domainEvent.VacancyId.Value);
        await _vacancyListingReadModelRepository.Remove(domainEvent.VacancyId.Value);
        await _vacancyApplicationsReadModelRepository.Remove(domainEvent.VacancyId.Value);

        await _unitOfWork.SaveChangesAsync();
    }
}
