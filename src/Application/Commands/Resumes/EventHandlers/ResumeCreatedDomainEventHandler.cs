using Application.Services;
using Domain.Abstraction.Interfaces;
using Domain.Contexts.ResumePostingContext.Events;
using Domain.Repos.ReadModels;
using Domain.Shared.Events;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Resumes.EventHandlers;

internal sealed class ResumeCreatedDomainEventHandler : IDomainEventHandler<ResumeCreatedDomainEvent>
{
    private readonly IUserResumesReadModelService _userResumesReadModelService;
    private readonly IUserResumesReadModelRepository _userResumesReadModelRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ResumeCreatedDomainEventHandler> _logger;

    public ResumeCreatedDomainEventHandler(
        IUserResumesReadModelService userResumesReadModelService,
        IUserResumesReadModelRepository userResumesReadModelRepository,
        IUnitOfWork unitOfWork,
        ILogger<ResumeCreatedDomainEventHandler> logger)
    {
        _userResumesReadModelService = userResumesReadModelService;
        _userResumesReadModelRepository = userResumesReadModelRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task Handle(ResumeCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var readModel = await _userResumesReadModelService.GenerateReadModelAsync(domainEvent.ResumeId, cancellationToken);
        if (readModel is not null)
        {
            _userResumesReadModelRepository.Add(readModel);
            await _unitOfWork.SaveChangesAsync();
        }
        else
        {
            _logger.LogError("Failed to generate read model for ResumeCreatedDomainEvent with ResumeId: {ResumeId}", domainEvent.ResumeId);
        }
    }
}
