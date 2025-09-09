using Application.Abstractions.Messaging;
using Domain.Contexts.ResumePostingContext.IDs;
using Domain.Repos.Resumes;
using Domain.Shared.ErrorHandling;

namespace Application.Queries.Resumes.GetResume;

public record class GetResumeQuery(Guid ResumeId) : IQuery<GetResumeQueryResponse>;

public record class GetResumeQueryResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Country,
    string City,
    string? Region,
    string? District,
    decimal? Latitude,
    decimal? Longitude,
    string Email,
    string PhoneNumber,
    string Title,
    decimal SalaryAmount,
    string SalaryCurrency,
    string SkillsDescriptionMarkdown,
    ICollection<string> EmploymentTypes,
    ICollection<string> WorkArrangements,
    ICollection<WorkExperienceDto> WorkExperiences,
    ICollection<EducationDto> Educations,
    ICollection<LanguageSkillDto> Languages);

public record EducationDto(
    Guid Id,
    string InstitutionName,
    string Degree,
    string FieldOfStudy,
    DateTime StartDate,
    DateTime? EndDate);

public record WorkExperienceDto(
    Guid Id,
    string CompanyName,
    string Position,
    string DescriptionMarkdown,
    DateTime StartDate,
    DateTime? EndDate);

public record LanguageSkillDto(
    Guid Id,
    string Language,
    string ProficiencyLevel);

internal sealed class GetResumeQueryHandler : IQueryHandler<GetResumeQuery, GetResumeQueryResponse>
{
    private readonly IResumeRepository _resumeRepository;

    public GetResumeQueryHandler(IResumeRepository resumeRepository)
    {
        _resumeRepository = resumeRepository;
    }

    public async Task<Result<GetResumeQueryResponse>> Handle(GetResumeQuery query, CancellationToken cancellationToken)
    {
        var resume = await _resumeRepository.GetByIdAsync(new ResumeId(query.ResumeId), cancellationToken);
        if (resume == null)
        {
            return Result<GetResumeQueryResponse>.Failure(Error.NotFound("Resume.NotFound", "The resume was not found"));
        }

        return Result<GetResumeQueryResponse>.Success(new GetResumeQueryResponse(
            resume.Id.Value,
            resume.PersonalInfo.FirstName,
            resume.PersonalInfo.LastName,
            resume.Location.Country,
            resume.Location.City,
            resume.Location.Region,
            resume.Location.District,
            resume.Location.Latitude,
            resume.Location.Longitude,
            resume.ContactInfo.Email.Address,
            resume.ContactInfo.PhoneNumber.Number,
            resume.DesiredPosition.Title,
            resume.SalaryExpectation.Value,
            resume.SalaryExpectation.Currency,
            resume.SkillsDescription.Markdown,
            resume.EmploymentTypes.Select(et => et.ToString()).ToList(),
            resume.WorkArrangements.Select(wa => wa.ToString()).ToList(),
            resume.WorkExperiences.Select(we => new WorkExperienceDto(
                we.Id.Value,
                we.CompanyName,
                we.Position,
                we.Description.Markdown,
                we.DateRange.StartDate,
                we.DateRange.EndDate)).ToList(),
            resume.Educations.Select(ed => new EducationDto(
                ed.Id.Value,
                ed.InstitutionName,
                ed.Degree,
                ed.FieldOfStudy,
                ed.DateRange.StartDate,
                ed.DateRange.EndDate)).ToList(),
            resume.Languages.Select(ls => new LanguageSkillDto(
                ls.Id.Value,
                ls.Language,
                ls.ProficiencyLevel.ToString())).ToList()));
    }
}
