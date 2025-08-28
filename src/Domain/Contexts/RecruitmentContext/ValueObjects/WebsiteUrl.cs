using Domain.Shared.ErrorHandling;
using Domain.Shared.ValueObjects;

namespace Domain.Contexts.RecruitmentContext.ValueObjects;

public class WebsiteUrl : Url
{
    private WebsiteUrl(string url)
        : base(url)
    {
    }

    public static WebsiteUrl None => new(string.Empty);

    public static Result<WebsiteUrl> Create(string url)
    {
        var validationResult = ValidateUrl(url);
        if (validationResult.IsFailure)
        {
            return Result<WebsiteUrl>.Failure(validationResult.Error);
        }

        return Result<WebsiteUrl>.Success(new WebsiteUrl(url));
    }
}