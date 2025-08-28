using Domain.Abstraction;

namespace Domain.Contexts.IdentityContext.Enums;

public class UserRole : Enumeration<UserRole>
{
    public static readonly UserRole JobSeeker = new("JOB_SEEKER", "Job Seeker");
    public static readonly UserRole Employer = new("EMPLOYER", "Employer");
    public static readonly UserRole Admin = new("ADMIN", "Admin");

    private UserRole(string code, string name)
        : base(code, name)
    {
    }
}
