namespace Application.Commands.VacancyApplications;

public interface IApplicationInfo
{
    bool Anonymous { get; }
}

internal sealed record class AuthenticatedApplicationInfo(Guid UserId) : IApplicationInfo
{
    public bool Anonymous => false;
}

internal sealed record class AnonymousApplicationInfo(string Email, string PhoneNumber, string PhoneNumberRegionCode) : IApplicationInfo
{
    public bool Anonymous => true;
}
