using Domain.Abstraction.Interfaces;
using Domain.Contexts.JobPostingContext.Events;
using Domain.Repos.ReadModels;
using Domain.Shared.Events;

namespace Application.Commands.Vacancies.EventHandler;

internal sealed class VacancyUpdatedDomainEventHandler : IDomainEventHandler<VacancyUpdatedDomainEvent>
{
    private readonly ICompanyVacanciesReadModelRepository _companyVacanciesReadModelRepository;
    private readonly IVacancyListingReadModelRepository _vacancyListingReadModelRepository;
    private readonly IUnitOfWork _unitOfWork;

    public VacancyUpdatedDomainEventHandler(
        ICompanyVacanciesReadModelRepository companyVacanciesReadModelRepository,
        IVacancyListingReadModelRepository vacancyListingReadModelRepository,
        IUnitOfWork unitOfWork)
    {
        _companyVacanciesReadModelRepository = companyVacanciesReadModelRepository;
        _vacancyListingReadModelRepository = vacancyListingReadModelRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(VacancyUpdatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await _companyVacanciesReadModelRepository.Update(domainEvent.VacancyId.Value);
        await _companyVacanciesReadModelRepository.Update(domainEvent.VacancyId.Value);
        await _unitOfWork.SaveChangesAsync();
    }
}
