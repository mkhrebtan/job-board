using Domain.Abstraction;
using Domain.Shared.ErrorHandling;

namespace Domain.Contexts.ApplicationContext.ValueObjects;

public class CoverLetter : ValueObject
{
    private CoverLetter(string content)
    {
        Content = content;
    }

    public string Content { get; private init; }

    public static Result<CoverLetter> Create(string content)
    {
        if (content is null)
        {
            return Result<CoverLetter>.Failure(new Error("CoverLetter.NullContent", "Cover letter content cannot be null."));
        }

        var letter = new CoverLetter(content);
        return Result<CoverLetter>.Success(letter);
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        throw new NotImplementedException();
    }
}
