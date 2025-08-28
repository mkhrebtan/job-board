using Domain.Abstraction;

namespace Domain.Contexts.ResumePostingContext.Enums;

public class LanguageLevel : Enumeration<LanguageLevel>
{
    public static readonly LanguageLevel A1 = new("A1", "A1 - Beginner");
    public static readonly LanguageLevel A2 = new("A2", "A2 - Elementary");
    public static readonly LanguageLevel B1 = new("B1", "B1 - Intermediate");
    public static readonly LanguageLevel B2 = new("B2", "B2 - Upper Intermediate");
    public static readonly LanguageLevel C1 = new("C1", "C1 - Advanced");
    public static readonly LanguageLevel C2 = new("C2", "C2 - Proficient");

    private LanguageLevel(string code, string name)
        : base(code, name)
    {
    }
}
