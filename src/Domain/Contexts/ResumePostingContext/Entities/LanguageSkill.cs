using Domain.Abstraction;
using Domain.Contexts.ResumePostingContext.Enums;
using Domain.Contexts.ResumePostingContext.IDs;
using Domain.Shared.ErrorHandling;

namespace Domain.Contexts.ResumePostingContext.Entities;

public class LanguageSkill : Entity<LanguageId>
{
    private LanguageSkill(string language, LanguageLevel proficiencyLevel)
        : base(new LanguageId())
    {
        Language = language;
        ProficiencyLevel = proficiencyLevel;
    }

    public string Language { get; private set; }

    public LanguageLevel ProficiencyLevel { get; private set; }

    public static Result<LanguageSkill> Create(string language, LanguageLevel proficiencyLevel)
    {
        if (string.IsNullOrWhiteSpace(language))
        {
            return Result<LanguageSkill>.Failure(new Error("LanguageSkill.InvalidLanguage", "Language cannot be null or empty."));
        }

        var skill = new LanguageSkill(language.Trim(), proficiencyLevel);
        return Result<LanguageSkill>.Success(skill);
    }
}
