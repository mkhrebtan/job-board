using Application.Abstractions.Messaging;
using Domain.Abstraction.Interfaces;
using Domain.Contexts.ResumePostingContext.IDs;
using Domain.Repos.Resumes;
using Domain.Shared.ErrorHandling;

namespace Application.Commands.Resumes.Updates.PublishResume;

internal sealed class PublishResumeCommandHandler : ICommandHandler<PublishResumeCommand>
{
    private readonly IResumeRepository _resumeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public PublishResumeCommandHandler(IResumeRepository resumeRepository, IUnitOfWork unitOfWork)
    {
        _resumeRepository = resumeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(PublishResumeCommand command, CancellationToken cancellationToken = default)
    {
        var resume = await _resumeRepository.GetByIdAsync(new ResumeId(command.Id), cancellationToken);
        if (resume is null)
        {
            return Result.Failure(Error.NotFound("Resume.NotFound", "Resume not found."));
        }

        var draftResult = resume.Publish();
        if (draftResult.IsFailure)
        {
            return Result.Failure(draftResult.Error);
        }

        _resumeRepository.Update(resume);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
