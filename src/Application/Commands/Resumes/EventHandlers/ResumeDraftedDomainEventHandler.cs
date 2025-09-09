using Domain.Abstraction.Interfaces;
using Domain.Contexts.ResumePostingContext.Events;
using Domain.Repos.ReadModels;
using Domain.Shared.Events;

namespace Application.Commands.Resumes.EventHandlers;

internal sealed class ResumeDraftedDomainEventHandler : IDomainEventHandler<ResumeDraftedDomainEvent>
{
    private readonly IResumeListingReadModelRepository _resumeListingReadModelRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ResumeDraftedDomainEventHandler(IResumeListingReadModelRepository resumeListingReadModelRepository, IUnitOfWork unitOfWork)
    {
        _resumeListingReadModelRepository = resumeListingReadModelRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ResumeDraftedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await _resumeListingReadModelRepository.Remove(domainEvent.ResumeId.Value);
        await _unitOfWork.SaveChangesAsync();
    }
}
