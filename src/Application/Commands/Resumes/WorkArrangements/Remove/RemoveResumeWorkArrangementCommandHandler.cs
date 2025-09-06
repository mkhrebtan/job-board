using Application.Abstractions.Messaging;
using Application.Common.Helpers;
using Domain.Abstraction.Interfaces;
using Domain.Contexts.ResumePostingContext.Enums;
using Domain.Contexts.ResumePostingContext.IDs;
using Domain.Repos.Resumes;
using Domain.Shared.ErrorHandling;

namespace Application.Commands.Resumes.WorkArrangements.Remove;

internal sealed class RemoveResumeWorkArrangementCommandHandler : ICommandHandler<RemoveResumeWorkArrangementCommand>
{
    private readonly IResumeRepository _resumeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveResumeWorkArrangementCommandHandler(IResumeRepository resumeRepository, IUnitOfWork unitOfWork)
    {
        _resumeRepository = resumeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RemoveResumeWorkArrangementCommand command, CancellationToken cancellationToken = default)
    {
        var resume = await _resumeRepository.GetByIdAsync(new ResumeId(command.Id), cancellationToken);
        if (resume is null)
        {
            return Result.Failure(Error.NotFound("Resume.NotFound", "The resume was not found."));
        }

        if (!Helpers.TryParseSmartEnum(command.WorkArrangements, "CreateResumeCommand.InvalidWorkArrangement", "Invalid work arrangement", out ICollection<WorkArrangement> workArrangements, out Error error))
        {
            return Result.Failure(error);
        }

        foreach (var workArrangement in workArrangements)
        {
            var result = resume.RemoveWorkArrangement(workArrangement);
            if (result.IsFailure)
            {
                return Result.Failure(result.Error);
            }
        }

        _resumeRepository.Update(resume);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}