using Domain.Abstraction.Interfaces;
using Domain.Contexts.JobPostingContext.Events;
using Domain.Repos.ReadModels;
using Domain.Shared.Events;

namespace Application.Commands.Vacancies.EventHandler;

internal sealed class VacancyArchivedDomainEventHandler : IDomainEventHandler<VacancyArchivedDomainEvent>
{
    private readonly ICompanyVacanciesReadModelRepository _companyVacanciesReadModelRepository;
    private readonly IVacancyListingReadModelRepository _vacancyListingReadModelRepository;
    private readonly IUnitOfWork _unitOfWork;

    public VacancyArchivedDomainEventHandler(
        ICompanyVacanciesReadModelRepository companyVacanciesReadModelRepository,
        IVacancyListingReadModelRepository vacancyListingReadModelRepository,
        IUnitOfWork unitOfWork)
    {
        _companyVacanciesReadModelRepository = companyVacanciesReadModelRepository;
        _vacancyListingReadModelRepository = vacancyListingReadModelRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(VacancyArchivedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await _companyVacanciesReadModelRepository.Update(domainEvent.VacancyId.Value);
        await _vacancyListingReadModelRepository.Update(domainEvent.VacancyId.Value);
        await _unitOfWork.SaveChangesAsync();
    }
}
