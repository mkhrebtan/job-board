using Domain.Abstraction;
using Domain.Shared.ErrorHandling;

namespace Domain.Shared.ValueObjects;

public abstract class Url : ValueObject
{
    public const string UrlPattern = @"^(http|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?$";

    protected Url(string url)
    {
        Value = url;
    }

    public string Value { get; private set; }

    protected static Result ValidateUrl(string url, string pattern)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return Result.Failure(new Error("Url.Empty", "URL cannot be null or empty."));
        }

        if (!System.Text.RegularExpressions.Regex.IsMatch(url, pattern))
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
