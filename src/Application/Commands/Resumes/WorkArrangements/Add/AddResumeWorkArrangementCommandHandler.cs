using Application.Abstractions.Messaging;
using Application.Common.Helpers;
using Domain.Abstraction.Interfaces;
using Domain.Contexts.ResumePostingContext.Enums;
using Domain.Repos.Resumes;
using Domain.Shared.ErrorHandling;

namespace Application.Commands.Resumes.WorkArrangements.Add;

internal sealed class AddResumeWorkArrangementCommandHandler : ICommandHandler<AddResumeWorkArrangementCommand>
{
    private readonly IResumeRepository _resumeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddResumeWorkArrangementCommandHandler(IResumeRepository resumeRepository, IUnitOfWork unitOfWork)
    {
        _resumeRepository = resumeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(AddResumeWorkArrangementCommand command, CancellationToken cancellationToken = default)
    {
        var resume = await _resumeRepository.GetByIdAsync(command.Id, cancellationToken);
        if (resume is null)
        {
            return Result.Failure(Error.NotFound("Resume.NotFound", "The resume was not found."));
        }

        if (!Helpers.TryParseSmartEnum(command.WorkArrangements, "AddResumeWorkArrangementCommand.InvalidWorkArrangement", "Invalid work arrangement", out ICollection<WorkArrangement> workArrangements, out Error error))
        {
            return Result.Failure(error);
        }

        foreach (var workArrangement in workArrangements)
        {
            var result = resume.AddWorkArrangement(workArrangement);
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