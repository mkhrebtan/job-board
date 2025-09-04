using Domain.Abstraction;
using Domain.Shared.ErrorHandling;

namespace Domain.Contexts.ResumePostingContext.ValueObjects;

public class DesiredPosition : ValueObject
{
    public const int MaxLength = 100;

    private DesiredPosition(string title)
    {
        Title = title;
    }

    public string Title { get; private init; }

    public static Result<DesiredPosition> Create(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return Result<DesiredPosition>.Failure(Error.Problem("DesiredPosition.InvalidTitle", "Desired position title cannot be null or empty."));
        }

        if (title.Length > MaxLength)
        {
            return Result<DesiredPosition>.Failure(Error.Problem("DesiredPosition.TitleTooLong", $"Desired position title cannot exceed {MaxLength} characters."));
        }

        var position = new DesiredPosition(title.Trim());
        return Result<DesiredPosition>.Success(position);
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Title;
    }
}
