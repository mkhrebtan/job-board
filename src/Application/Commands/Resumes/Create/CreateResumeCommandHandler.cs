using Application.Abstractions.Messaging;
using Application.Common.Helpers;
using Domain.Abstraction;
using Domain.Abstraction.Interfaces;
using Domain.Contexts.IdentityContext.IDs;
using Domain.Contexts.JobPostingContext.ValueObjects;
using Domain.Contexts.ResumePostingContext.Enums;
using Domain.Contexts.ResumePostingContext.ValueObjects;
using Domain.Repos.Resumes;
using Domain.Repos.Users;
using Domain.Services;
using Domain.Shared.ErrorHandling;
using Domain.Shared.ValueObjects;

namespace Application.Commands.Resumes.Create;

internal sealed class CreateResumeCommandHandler : ICommandHandler<CreateResumeCommand, CreateResumeCommandResponse>
{
    private readonly IResumeRepository _resumeRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMarkdownParser _markdownParser;
    private readonly IUnitOfWork _unitOfWork;

    public CreateResumeCommandHandler(IResumeRepository resumeRepository, IUserRepository userRepository, IMarkdownParser markdownParser, IUnitOfWork unitOfWork)
    {
        _resumeRepository = resumeRepository;
        _userRepository = userRepository;
        _markdownParser = markdownParser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreateResumeCommandResponse>> Handle(CreateResumeCommand command, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(new UserId(command.SeekerId), cancellationToken);
        if (user is null)
        {
            return Result<CreateResumeCommandResponse>.Failure(Error.NotFound("User.NotFound", "The user was not found."));
        }

        if (!Helpers.TryCreateVO(() => PersonalInfo.Create(command.FirstName, command.LastName), out PersonalInfo personalInfo, out Error error))
        {
            return Result<CreateResumeCommandResponse>.Failure(error);
        }

        if (!Helpers.TryCreateVO(() => Location.Create(command.Country, command.City, command.Region, command.District, latitude: command.Latitude, longitude: command.Longitude), out Location location, out error))
        {
            return Result<CreateResumeCommandResponse>.Failure(error);
        }

        if (!Helpers.TryCreateVO(() => Email.Create(command.Email), out Email email, out error))
        {
            return Result<CreateResumeCommandResponse>.Failure(error);
        }

        if (!Helpers.TryCreateVO(() => PhoneNumber.Create(command.PhoneNumber, command.PhoneNumberRegionCode), out PhoneNumber phoneNumber, out error))
        {
            return Result<CreateResumeCommandResponse>.Failure(error);
        }

        if (!Helpers.TryCreateVO(() => ContactInfo.Create(email, phoneNumber), out ContactInfo contactInfo, out error))
        {
            return Result<CreateResumeCommandResponse>.Failure(error);
        }

        if (!Helpers.TryCreateVO(() => DesiredPosition.Create(command.DesiredPositionTitle), out DesiredPosition position, out error))
        {
            return Result<CreateResumeCommandResponse>.Failure(error);
        }

        if (!Helpers.TryCreateVO(() => Money.Create(command.SalaryAmount, command.SalaryCurrency), out Money salary, out error))
        {
            return Result<CreateResumeCommandResponse>.Failure(error);
        }

        if (!Helpers.TryCreateVO(() => RichTextContent.Create(command.SkillsDescripitonMarkdown, _markdownParser), out RichTextContent description, out error))
        {
            return Result<CreateResumeCommandResponse>.Failure(error);
        }

        if (!Helpers.TryParseSmartEnum(command.EmploymentTypes, "CreateResumeCommand.InvalidEmploymentType", "Invalid employment type", out ICollection<EmploymentType> employmentTypes, out error))
        {
            return Result<CreateResumeCommandResponse>.Failure(error);
        }

        if (!Helpers.TryParseSmartEnum(command.WorkArrangements, "CreateResumeCommand.InvalidWorkArrangement", "Invalid work arrangement", out ICollection<WorkArrangement> workArrangements, out error))
        {
            return Result<CreateResumeCommandResponse>.Failure(error);
        }

        var resumeResult = ResumeService.CreateResume(
            user,
            personalInfo,
            location,
            contactInfo,
            position,
            salary,
            description,
            employmentTypes,
            workArrangements);
        if (resumeResult.IsFailure)
        {
            return Result<CreateResumeCommandResponse>.Failure(resumeResult.Error);
        }

        var resume = resumeResult.Value;
        foreach (var workExp in command.WorkExpetiences)
        {
            var dateRangeResult = workExp.EndDate is null ? DateRange.Create(workExp.StartDate) : DateRange.Create(workExp.StartDate, (DateTime)workExp.EndDate);
            if (dateRangeResult.IsFailure)
            {
                return Result<CreateResumeCommandResponse>.Failure(dateRangeResult.Error);
            }

            Result<RichTextContent> workDescriptionResult = default!;
            if (!string.IsNullOrEmpty(workExp.DescriptionMarkdown))
            {
                workDescriptionResult = RichTextContent.Create(workExp.DescriptionMarkdown, _markdownParser);
                if (workDescriptionResult.IsFailure)
                {
                    return Result<CreateResumeCommandResponse>.Failure(workDescriptionResult.Error);
                }
            }

            var workExpResult = resume.AddWorkExperience(
                workExp.CompanyName,
                workExp.Position,
                dateRangeResult.Value,
                workDescriptionResult.Value);
            if (workExpResult.IsFailure)
            {
                return Result<CreateResumeCommandResponse>.Failure(workExpResult.Error);
            }
        }

        foreach (var edu in command.Educations)
        {
            var dateRangeResult = edu.EndDate is null ? DateRange.Create(edu.StartDate) : DateRange.Create(edu.StartDate, (DateTime)edu.EndDate);
            if (dateRangeResult.IsFailure)
            {
                return Result<CreateResumeCommandResponse>.Failure(dateRangeResult.Error);
            }

            var eduResult = resume.AddEducation(
                edu.InstitutionName,
                edu.Degree,
                edu.FieldOfStudy,
                dateRangeResult.Value);
            if (eduResult.IsFailure)
            {
                return Result<CreateResumeCommandResponse>.Failure(eduResult.Error);
            }
        }

        foreach (var lang in command.Languages)
        {
            var proficiencyResult = LanguageLevel.FromCode(lang.ProficiencyLevel);
            if (proficiencyResult is null)
            {
                return Result<CreateResumeCommandResponse>.Failure(Error.Validation("CreateResumeCommand.InvalidLanguageProficiency", $"Invalid language proficiency: '{lang.ProficiencyLevel}'."));
            }

            var langResult = resume.AddLanguage(lang.Language, proficiencyResult);
            if (langResult.IsFailure)
            {
                return Result<CreateResumeCommandResponse>.Failure(langResult.Error);
            }
        }

        _resumeRepository.Add(resume);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<CreateResumeCommandResponse>.Success(new CreateResumeCommandResponse(resume.Id.Value));
    }
}