using Domain.Shared.ErrorHandling;
using Domain.Shared.ValueObjects;

namespace Domain.Contexts.RecruitmentContext.ValueObjects;

public class LogoUrl : Url
{
    private LogoUrl(string url)
        : base(url)
    {
    }

    public static LogoUrl None => new LogoUrl(string.Empty);

    public static Result<LogoUrl> Create(string url)
    {
        var validationResult = ValidateUrl(url);
        if (validationResult.IsFailure)
        {
            return Result<LogoUrl>.Failure(validationResult.Error);
        }

        return Result<LogoUrl>.Success(new LogoUrl(url));
    }
}
