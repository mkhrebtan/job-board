using Application.Abstractions.Messaging;
using Application.Commands.Resumes.Dtos;
using Domain.Abstraction.Interfaces;
using Domain.Contexts.ResumePostingContext.ValueObjects;
using Domain.Repos.Resumes;
using Domain.Shared.ErrorHandling;
using Domain.Shared.ValueObjects;

namespace Application.Commands.Resumes.WorkExperiences.Add;

public record AddResumeWorkExperienceCommand(Guid Id, WorkExperienceDto WorkExperience) : ICommand;

internal sealed class AddResumeWorkExperienceCommandHandler : ICommandHandler<AddResumeWorkExperienceCommand>
{
    private readonly IResumeRepository _resumeRepository;
    private readonly IMarkdownParser _markdownParser;
    private readonly IUnitOfWork _unitOfWork;

    public AddResumeWorkExperienceCommandHandler(IResumeRepository resumeRepository, IMarkdownParser markdownParser, IUnitOfWork unitOfWork)
    {
        _resumeRepository = resumeRepository;
        _markdownParser = markdownParser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(AddResumeWorkExperienceCommand command, CancellationToken cancellationToken = default)
    {
        var resume = await _resumeRepository.GetByIdAsync(command.Id, cancellationToken);
        if (resume is null)
        {
            return Result.Failure(Error.NotFound("Resume.NotFound", "The resume was not found."));
        }

        var dateRangeResult = command.WorkExperience.EndDate is null ? DateRange.Create(command.WorkExperience.StartDate) : DateRange.Create(command.WorkExperience.StartDate, (DateTime)command.WorkExperience.EndDate);
        if (dateRangeResult.IsFailure)
        {
            return Result.Failure(dateRangeResult.Error);
        }

        Result<RichTextContent> workDescriptionResult = default!;
        if (!string.IsNullOrEmpty(command.WorkExperience.DescriptionMarkdown))
        {
            workDescriptionResult = RichTextContent.Create(command.WorkExperience.DescriptionMarkdown, _markdownParser);
            if (workDescriptionResult.IsFailure)
            {
                return Result.Failure(workDescriptionResult.Error);
            }
        }

        var workExpResult = resume.AddWorkExperience(
            command.WorkExperience.CompanyName,
            command.WorkExperience.Position,
            dateRangeResult.Value,
            workDescriptionResult.Value);
        if (workExpResult.IsFailure)
        {
            return Result.Failure(workExpResult.Error);
        }

        _resumeRepository.Update(resume);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}