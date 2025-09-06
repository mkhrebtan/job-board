using Application.Abstractions.Messaging;
using Application.Common.Helpers;
using Domain.Abstraction.Interfaces;
using Domain.Contexts.ResumePostingContext.IDs;
using Domain.Contexts.ResumePostingContext.ValueObjects;
using Domain.Repos.Resumes;
using Domain.Shared.ErrorHandling;

namespace Application.Commands.Resumes.Updates.UpdateDesiredPosition;

internal sealed class UpdateResumeDesiredPositionCommandHandler : ICommandHandler<UpdateResumeDesiredPositionCommand>
{
    private readonly IResumeRepository _resumeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateResumeDesiredPositionCommandHandler(IResumeRepository resumeRepository, IUnitOfWork unitOfWork)
    {
        _resumeRepository = resumeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateResumeDesiredPositionCommand command, CancellationToken cancellationToken = default)
    {
        var resume = await _resumeRepository.GetByIdAsync(new ResumeId(command.Id), cancellationToken);
        if (resume is null)
        {
            return Result.Failure(Error.NotFound("Resume.NotFound", "The resume was not found."));
        }

        if (!Helpers.TryCreateVO(() => DesiredPosition.Create(command.DesiredPositionTitle), out DesiredPosition position, out Error error))
        {
            return Result.Failure(error);
        }

        var updateResult = resume.UpdateDesiredPosition(position);
        if (updateResult.IsFailure)
        {
            return Result.Failure(updateResult.Error);
        }

        _resumeRepository.Update(resume);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}