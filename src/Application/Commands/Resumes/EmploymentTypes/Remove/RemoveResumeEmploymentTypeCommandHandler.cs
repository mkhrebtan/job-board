using Application.Abstractions.Messaging;
using Application.Common.Helpers;
using Domain.Abstraction.Interfaces;
using Domain.Contexts.ResumePostingContext.Enums;
using Domain.Contexts.ResumePostingContext.IDs;
using Domain.Repos.Resumes;
using Domain.Shared.ErrorHandling;

namespace Application.Commands.Resumes.EmploymentTypes.Remove;

internal sealed class RemoveResumeEmploymentTypeCommandHandler : ICommandHandler<RemoveResumeEmploymentTypeCommand>
{
    private readonly IResumeRepository _resumeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveResumeEmploymentTypeCommandHandler(IResumeRepository resumeRepository, IUnitOfWork unitOfWork)
    {
        _resumeRepository = resumeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RemoveResumeEmploymentTypeCommand command, CancellationToken cancellationToken = default)
    {
        var resume = await _resumeRepository.GetByIdAsync(new ResumeId(command.Id), cancellationToken);
        if (resume is null)
        {
            return Result.Failure(Error.NotFound("Resume.NotFound", "The resume was not found."));
        }

        if (!Helpers.TryParseSmartEnum(command.EmploymentTypes, "CreateResumeCommand.InvalidEmploymentType", "Invalid employment type", out ICollection<EmploymentType> employmentTypes, out Error error))
        {
            return Result.Failure(error);
        }

        foreach (var employmentType in employmentTypes)
        {
            var result = resume.RemoveEmploymentType(employmentType);
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