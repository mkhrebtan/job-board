using Domain.Abstraction.Interfaces;
using Domain.Contexts.ResumePostingContext.Events;
using Domain.Repos.ReadModels;
using Domain.Shared.Events;

namespace Application.Commands.Resumes.EventHandlers;

internal sealed class ResumeUpdatedDomainEventHandler : IDomainEventHandler<ResumeUpdateDomainEvent>
{
    private readonly IUserResumesReadModelRepository _userResumesReadModelRepository;
    private readonly IResumeListingReadModelRepository _resumeListingReadModelRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ResumeUpdatedDomainEventHandler(
        IUserResumesReadModelRepository userResumesReadModelRepository,
        IResumeListingReadModelRepository resumeListingReadModelRepository,
        IUnitOfWork unitOfWork)
    {
        _userResumesReadModelRepository = userResumesReadModelRepository;
        _resumeListingReadModelRepository = resumeListingReadModelRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ResumeUpdateDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await _userResumesReadModelRepository.Update(domainEvent.ResumeId.Value);
        await _resumeListingReadModelRepository.Update(domainEvent.ResumeId.Value);

        await _unitOfWork.SaveChangesAsync();
    }
}
