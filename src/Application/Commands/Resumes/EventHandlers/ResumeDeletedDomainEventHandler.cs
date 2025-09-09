using Domain.Abstraction.Interfaces;
using Domain.Contexts.ResumePostingContext.Events;
using Domain.Repos.ReadModels;
using Domain.Shared.Events;

namespace Application.Commands.Resumes.EventHandlers;

internal sealed class ResumeDeletedDomainEventHandler : IDomainEventHandler<ResumeDeletedDomainEvent>
{
    private readonly IUserResumesReadModelRepository _userResumesReadModelRepository;
    private readonly IResumeListingReadModelRepository _resumeListingReadModelRepository;
    private readonly IVacancyApplicationsReadModelRepository _vacancyApplicationsReadModelRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ResumeDeletedDomainEventHandler(
        IUserResumesReadModelRepository userResumesReadModelRepository,
        IResumeListingReadModelRepository resumeListingReadModelRepository,
        IVacancyApplicationsReadModelRepository vacancyApplicationsReadModelRepository,
        IUnitOfWork unitOfWork)
    {
        _userResumesReadModelRepository = userResumesReadModelRepository;
        _resumeListingReadModelRepository = resumeListingReadModelRepository;
        _vacancyApplicationsReadModelRepository = vacancyApplicationsReadModelRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ResumeDeletedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await _userResumesReadModelRepository.Remove(domainEvent.ResumeId.Value);
        await _resumeListingReadModelRepository.Remove(domainEvent.ResumeId.Value);
        await _vacancyApplicationsReadModelRepository.RemoveByResume(domainEvent.ResumeId.Value);

        await _unitOfWork.SaveChangesAsync();
    }
}
