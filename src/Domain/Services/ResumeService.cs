using Domain.Contexts.IdentityContext.Aggregates;
using Domain.Contexts.IdentityContext.Enums;
using Domain.Contexts.JobPostingContext.ValueObjects;
using Domain.Contexts.ResumePostingContext.Aggregates;
using Domain.Contexts.ResumePostingContext.Enums;
using Domain.Contexts.ResumePostingContext.ValueObjects;
using Domain.Shared.ErrorHandling;
using Domain.Shared.ValueObjects;

namespace Domain.Services;

public sealed class ResumeService
{
    private ResumeService()
    {
    }

    public static Result<Resume> CreateResume(
        User user,
        PersonalInfo personalInfo,
        Location location,
        ContactInfo contactInfo,
        DesiredPosition desiredPosition,
        Money salary,
        RichTextContent skillsDescription,
        ICollection<EmploymentType> employmentTypes,
        ICollection<WorkArrangement> workArrangements)
    {
        if (user.Role != UserRole.JobSeeker)
        {
            return Result<Resume>.Failure(Error.Problem("ResumeService.InvalidUserRole", $"Only users with the '{UserRole.JobSeeker.Name}' role can create resumes."));
        }

        return Resume.Create(
            user.Id,
            personalInfo,
            location,
            contactInfo,
            desiredPosition,
            salary,
            skillsDescription,
            employmentTypes,
            workArrangements);
    }
}
