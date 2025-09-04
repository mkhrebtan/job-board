using Domain.Abstraction;
using Domain.Shared.ErrorHandling;

namespace Domain.Contexts.ApplicationContext.ValueObjects;

public class CoverLetter : ValueObject
{
    public const int MaxLength = 10000;

    private CoverLetter(string content)
    {
        Content = content;
    }

    public string Content { get; private init; }

    public static Result<CoverLetter> Create(string content)
    {
        if (content is null)
        {
            return Result<CoverLetter>.Failure(Error.Problem("CoverLetter.NullContent", "Cover letter content cannot be null."));
        }

        if (content.Length > MaxLength)
        {
            return Result<CoverLetter>.Failure(Error.Problem("CoverLetter.ContentTooLong", $"Cover letter content cannot exceed {MaxLength} characters."));
        }

        var letter = new CoverLetter(content.Trim());
        return Result<CoverLetter>.Success(letter);
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Content;
    }
}
