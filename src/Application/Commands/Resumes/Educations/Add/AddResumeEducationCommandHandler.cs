using Application.Abstractions.Messaging;
using Domain.Abstraction.Interfaces;
using Domain.Contexts.ResumePostingContext.ValueObjects;
using Domain.Repos.Resumes;
using Domain.Shared.ErrorHandling;

namespace Application.Commands.Resumes.Educations.Add;

internal sealed class AddResumeEducationCommandHandler : ICommandHandler<AddResumeEducationCommand>
{
    private readonly IResumeRepository _resumeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddResumeEducationCommandHandler(IResumeRepository resumeRepository, IUnitOfWork unitOfWork)
    {
        _resumeRepository = resumeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(AddResumeEducationCommand command, CancellationToken cancellationToken)
    {
        var resume = await _resumeRepository.GetByIdAsync(command.Id, cancellationToken);
        if (resume is null)
        {
            return Result.Failure(Error.NotFound("Resume.NotFound", "The resume was not found."));
        }

        var dateRangeResult = command.Education.EndDate is null ? DateRange.Create(command.Education.StartDate) : DateRange.Create(command.Education.StartDate, (DateTime)command.Education.EndDate);
        if (dateRangeResult.IsFailure)
        {
            return Result.Failure(dateRangeResult.Error);
        }

        var eduResult = resume.AddEducation(
            command.Education.InstitutionName,
            command.Education.Degree,
            command.Education.FieldOfStudy,
            dateRangeResult.Value);
        if (eduResult.IsFailure)
        {
            return Result.Failure(eduResult.Error);
        }

        _resumeRepository.Update(resume);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}