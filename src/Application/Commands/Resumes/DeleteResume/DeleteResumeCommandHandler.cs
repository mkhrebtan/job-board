using Application.Abstractions.Messaging;
using Domain.Abstraction.Interfaces;
using Domain.Repos.Resumes;
using Domain.Shared.ErrorHandling;

namespace Application.Commands.Resumes.DeleteResume;

internal sealed class DeleteResumeCommandHandler : ICommandHandler<DeleteResumeCommand>
{
    private readonly IResumeRepository _resumeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteResumeCommandHandler(IResumeRepository resumeRepository, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _resumeRepository = resumeRepository;
    }

    public async Task<Result> Handle(DeleteResumeCommand command, CancellationToken cancellationToken = default)
    {
        var resume = await _resumeRepository.GetByIdAsync(command.Id, cancellationToken);
        if (resume is null)
        {
            return Result.Failure(Error.NotFound("DeleteResumeCommandHandler.ResumeNotFound", $"Resume with ID '{command.Id}' was not found."));
        }

        _resumeRepository.Remove(resume);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
