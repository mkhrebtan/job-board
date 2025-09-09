using Application.Services;
using Domain.Abstraction.Interfaces;
using Domain.Contexts.ResumePostingContext.Events;
using Domain.Repos.ReadModels;
using Domain.Shared.Events;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Resumes.EventHandlers;

internal sealed class ResumePublishedDomainEventHandler : IDomainEventHandler<ResumePublishedDomainEvent>
{
    private readonly IResumeListingReadModelRepository _resumeListingReadModelRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IResumeListingReadModelService _resumeListingReadModelService;
    private readonly ILogger<ResumePublishedDomainEventHandler> _logger;

    public ResumePublishedDomainEventHandler(
        IResumeListingReadModelRepository resumeListingReadModelRepository,
        IUnitOfWork unitOfWork,
        IResumeListingReadModelService resumeListingReadModelService,
        ILogger<ResumePublishedDomainEventHandler> logger)
    {
        _resumeListingReadModelRepository = resumeListingReadModelRepository;
        _unitOfWork = unitOfWork;
        _resumeListingReadModelService = resumeListingReadModelService;
        _logger = logger;
    }

    public async Task Handle(ResumePublishedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var readModel = await _resumeListingReadModelService.GenerateReadModelAsync(domainEvent.ResumeId);
        if (readModel != null)
        {
            _resumeListingReadModelRepository.Add(readModel);
            await _unitOfWork.SaveChangesAsync();
        }
        else
        {
            _logger.LogError("Failed to generate read model for ResumePublishedDomainEvent with ResumeId: {ResumeId}", domainEvent.ResumeId);
        }
    }
}
