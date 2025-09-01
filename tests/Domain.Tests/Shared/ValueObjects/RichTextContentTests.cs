using Domain.Abstraction.Interfaces;
using Domain.Shared.ValueObjects;
using Moq;

namespace Domain.Tests.Shared.ValueObjects;

public class RichTextContentTests
{
    private readonly Mock<IMarkdownParser> _mockParser;

    public RichTextContentTests()
    {
        _mockParser = new Mock<IMarkdownParser>();
    }

    [Fact]
    public void Create_WithValidMarkdown_ShouldReturnSuccess()
    {
        const string markdown = "# Hello World";
        const string expectedPlainText = "Hello World";
        _mockParser.Setup(x => x.ToPlainText(markdown)).Returns(expectedPlainText);

        var result = RichTextContent.Create(markdown, _mockParser.Object);

        Assert.True(result.IsSuccess);
        Assert.Equal(markdown, result.Value.Markdown);
        Assert.Equal(expectedPlainText, result.Value.PlainText);
        _mockParser.Verify(x => x.ToPlainText(markdown), Times.Once);
    }

    [Fact]
    public void Create_WithNullMarkdown_ShouldReturnFailure()
    {
        var result = RichTextContent.Create(null, _mockParser.Object);

        Assert.False(result.IsSuccess);
        Assert.NotNull(result.Error);
        _mockParser.Verify(x => x.ToPlainText(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void Empty_ShouldReturnEmptyRichTextContent()
    {
        var empty = RichTextContent.Empty;

        Assert.Equal(string.Empty, empty.Markdown);
        Assert.Equal(string.Empty, empty.PlainText);
    }

    [Fact]
    public void Equals_SameValues_ShouldReturnTrue()
    {
        const string markdown = "**Bold text**";
        const string plainText = "Bold text";
        _mockParser.Setup(x => x.ToPlainText(markdown)).Returns(plainText);

        var content1 = RichTextContent.Create(markdown, _mockParser.Object).Value;
        var content2 = RichTextContent.Create(markdown, _mockParser.Object).Value;

        Assert.True(content1.Equals(content2));
    }

    [Fact]
    public void Equals_DifferentValues_ShouldReturnFalse()
    {
        const string markdown1 = "**Bold text**";
        const string markdown2 = "*Italic text*";
        _mockParser.Setup(x => x.ToPlainText(markdown1)).Returns("Bold text");
        _mockParser.Setup(x => x.ToPlainText(markdown2)).Returns("Italic text");

        var content1 = RichTextContent.Create(markdown1, _mockParser.Object).Value;
        var content2 = RichTextContent.Create(markdown2, _mockParser.Object).Value;

        Assert.False(content1.Equals(content2));
    }
}
