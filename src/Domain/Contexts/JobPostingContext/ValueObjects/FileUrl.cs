using Domain.Shared.ErrorHandling;
using Domain.Shared.ValueObjects;

namespace Domain.Contexts.JobPostingContext.ValueObjects;

public class FileUrl : Url
{
    public FileUrl(string url)
        : base(url)
    {
    }

    public static Result<FileUrl> Create(string url)
    {
        var validationResult = ValidateUrl(url);
        if (validationResult.IsFailure)
        {
            return Result<FileUrl>.Failure(validationResult.Error);
        }

        return Result<FileUrl>.Success(new FileUrl(url));
    }
}
