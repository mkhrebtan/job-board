using Domain.Abstraction;

namespace Domain.Contexts.IdentityContext.Enums;

public class UserRole : Enumeration<UserRole>
{
    public static readonly UserRole JobSeeker = new("JOB_SEEKER", "Job Seeker");
    public static readonly UserRole CompanyEmployee = new("COMPANY_EMPLOYEE", "Company Employee");
    public static readonly UserRole CompanyAdmin = new("COMPANY_ADMIN", "Company Admin");
    public static readonly UserRole Admin = new("ADMIN", "Admin");

    private UserRole(string code, string name)
        : base(code, name)
    {
    }
}
