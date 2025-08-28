using Domain.Abstraction;
using Domain.Shared.ErrorHandling;

namespace Domain.Shared.ValueObjects;

public abstract class Url : ValueObject
{
    public const string UrlPattern = @"^https?:\\/\\/(?:www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b(?:[-a-zA-Z0-9()@:%_\\+.~#?&\\/=]*)$";

    protected Url(string url)
    {
        Value = url;
    }

    public string Value { get; private set; }

    protected static Result ValidateUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return Result.Failure(new Error("Url.Empty", "URL cannot be null or empty."));
        }

        if (!System.Text.RegularExpressions.Regex.IsMatch(url, UrlPattern))
        {
            return Result.Failure(new Error("Url.InvalidFormat", "URL format is invalid."));
        }

        return Result.Success();
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
