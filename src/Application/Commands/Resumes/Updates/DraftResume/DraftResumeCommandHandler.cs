using Application.Abstractions.Messaging;
using Domain.Abstraction.Interfaces;
using Domain.Contexts.ResumePostingContext.IDs;
using Domain.Repos.Resumes;
using Domain.Shared.ErrorHandling;

namespace Application.Commands.Resumes.Updates.DraftResume;

internal sealed class DraftResumeCommandHandler : ICommandHandler<DraftResumeCommand>
{
    private readonly IResumeRepository _resumeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DraftResumeCommandHandler(IResumeRepository resumeRepository, IUnitOfWork unitOfWork)
    {
        _resumeRepository = resumeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DraftResumeCommand command, CancellationToken cancellationToken = default)
    {
        var resume = await _resumeRepository.GetByIdAsync(new ResumeId(command.Id), cancellationToken);
        if (resume is null)
        {
            return Result.Failure(Error.NotFound("Resume.NotFound", "Resume not found."));
        }

        var draftResult = resume.Draft();
        if (draftResult.IsFailure)
        {
            return Result.Failure(draftResult.Error);
        }

        _resumeRepository.Update(resume);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
