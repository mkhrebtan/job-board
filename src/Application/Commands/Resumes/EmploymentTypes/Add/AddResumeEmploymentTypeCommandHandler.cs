using Application.Abstractions.Messaging;
using Application.Common.Helpers;
using Domain.Abstraction.Interfaces;
using Domain.Contexts.ResumePostingContext.Enums;
using Domain.Repos.Resumes;
using Domain.Shared.ErrorHandling;

namespace Application.Commands.Resumes.EmploymentTypes.Add;

internal sealed class AddResumeEmploymentTypeCommandHandler : ICommandHandler<AddResumeEmploymentTypeCommand>
{
    private readonly IResumeRepository _resumeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddResumeEmploymentTypeCommandHandler(IResumeRepository resumeRepository, IUnitOfWork unitOfWork)
    {
        _resumeRepository = resumeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(AddResumeEmploymentTypeCommand command, CancellationToken cancellationToken = default)
    {
        var resume = await _resumeRepository.GetByIdAsync(command.Id, cancellationToken);
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
            var result = resume.AddEmploymentType(employmentType);
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