using Domain.Shared.ErrorHandling;
using Domain.Shared.ValueObjects;

namespace Domain.Contexts.ApplicationContext.ValueObjects;

public class FileUrl : Url
{
    public FileUrl(string url)
        : base(url)
    {
    }

    public static Result<FileUrl> Create(string url)
    {
        var validationResult = ValidateUrl(url, UrlPattern);
        if (validationResult.IsFailure)
        {
            return Result<FileUrl>.Failure(validationResult.Error);
        }

        return Result<FileUrl>.Success(new FileUrl(url));
    }
}
