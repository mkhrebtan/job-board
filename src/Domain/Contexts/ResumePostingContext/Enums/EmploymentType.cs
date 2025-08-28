using Domain.Abstraction;

namespace Domain.Contexts.ResumePostingContext.Enums;

public class EmploymentType : Enumeration<EmploymentType>
{
    public static readonly EmploymentType FullTime = new("FT", "Full Time");
    public static readonly EmploymentType PartTime = new("PT", "Part Time");
    public static readonly EmploymentType Contract = new("CT", "Contract");
    public static readonly EmploymentType Internship = new("IN", "Internship");

    private EmploymentType(string code, string name)
        : base(code, name)
    {
    }
}