using Domain.Abstraction;

namespace Domain.Contexts.ResumePostingContext.Enums;

public class WorkArrangement : Enumeration<WorkArrangement>
{
    public static readonly WorkArrangement OnSite = new("OS", "On Site");
    public static readonly WorkArrangement Remote = new("RM", "Remote");
    public static readonly WorkArrangement Hybrid = new("HY", "Hybrid");
    public static readonly WorkArrangement Shift = new("SF", "Shift");

    private WorkArrangement(string code, string name)
        : base(code, name)
    {
    }
}
