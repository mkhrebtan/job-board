using Domain.Abstraction;
using Domain.Abstraction.Interfaces;
using Domain.Shared.ErrorHandling;

namespace Domain.Shared.ValueObjects;

public class RichTextContent : ValueObject
{
    public const int MaxLength = 50000;

    private RichTextContent(string markdown, string plainText)
    {
        Markdown = markdown;
        PlainText = plainText;
    }

    public static RichTextContent Empty => new(string.Empty, string.Empty);

    public string Markdown { get; private init; }

    public string PlainText { get; private init; }

    public static Result<RichTextContent> Create(string markdown, IMarkdownParser parser)
    {
        if (markdown is null)
        {
            return Result<RichTextContent>.Failure(new Error("RichTextContent.NullMarkdown", "Markdown content cannot be null."));
        }

        if (markdown.Length > MaxLength)
        {
            return Result<RichTextContent>.Failure(new Error("RichTextContent.MarkdownTooLong", $"Markdown content cannot exceed {MaxLength} characters."));
        }

        string plainText = parser.ToPlainText(markdown);
        return Result<RichTextContent>.Success(new RichTextContent(markdown, plainText));
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Markdown;
        yield return PlainText;
    }
}
