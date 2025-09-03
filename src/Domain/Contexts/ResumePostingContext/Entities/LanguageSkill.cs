using Domain.Abstraction;
using Domain.Contexts.ResumePostingContext.Enums;
using Domain.Contexts.ResumePostingContext.IDs;
using Domain.Shared.ErrorHandling;

namespace Domain.Contexts.ResumePostingContext.Entities;

public class LanguageSkill : Entity<LanguageId>
{
    public const int MaxLanguageLength = 50;

    private LanguageSkill(ResumeId resumeId, string language, LanguageLevel proficiencyLevel)
        : base(new LanguageId())
    {
        ResumeId = resumeId;
        Language = language;
        ProficiencyLevel = proficiencyLevel;
    }

    public ResumeId ResumeId { get; private set; }

    public string Language { get; private set; }

    public LanguageLevel ProficiencyLevel { get; private set; }

    public static Result<LanguageSkill> Create(ResumeId resumeId, string language, LanguageLevel proficiencyLevel)
    {
        if (resumeId == null || resumeId.Value == Guid.Empty)
        {
            return Result<LanguageSkill>.Failure(new Error("LanguageSkill.InvalidResumeId", "ResumeId cannot be null or empty."));
        }

        if (string.IsNullOrWhiteSpace(language))
        {
            return Result<LanguageSkill>.Failure(new Error("LanguageSkill.InvalidLanguage", "Language cannot be null or empty."));
        }

        if (language.Length > MaxLanguageLength)
        {
            return Result<LanguageSkill>.Failure(new Error("LanguageSkill.LanguageTooLong", $"Language cannot exceed {MaxLanguageLength} characters."));
        }

        var skill = new LanguageSkill(resumeId, language.Trim(), proficiencyLevel);
        return Result<LanguageSkill>.Success(skill);
    }
}
