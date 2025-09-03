using Domain.Contexts.ApplicationContext.ValueObjects;

namespace Domain.Tests.Contexts.ApplicationContext.ValueObjects;

public class CoverLetterTests
{
    [Fact]
    public void Create_WithValidContent_ShouldReturnSuccess()
    {
        var content = "Dear hiring manager, I am writing to express my interest...";

        var result = CoverLetter.Create(content);

        Assert.True(result.IsSuccess);
        Assert.Equal(result.Value.Content, content);
    }

    [Fact]
    public void Create_WithNullContent_ShouldReturnFailure()
    {
        string? content = null;

        var result = CoverLetter.Create(content!);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Create_WithEmptyString_ShouldReturnSuccess()
    {
        var content = string.Empty;

        var result = CoverLetter.Create(content);

        Assert.True(result.IsSuccess);
        Assert.Equal(result.Value.Content, content);
    }

    [Fact]
    public void Create_WithWhitespace_ShouldReturnSuccessAndSaveEmptyString()
    {
        var content = "   ";
        var expectedContent = string.Empty;

        var result = CoverLetter.Create(content);

        Assert.True(result.IsSuccess);
        Assert.Equal(result.Value.Content, expectedContent);
    }

    [Fact]
    public void Create_WithExceedingMaxLength_ShouldReturnFailure()
    {
        var content = new string('a', CoverLetter.MaxLength + 1);

        var result = CoverLetter.Create(content);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Equals_WithSameContent_ShouldReturnTrue()
    {
        var content = "Sample cover letter content";
        var coverLetter1 = CoverLetter.Create(content).Value;
        var coverLetter2 = CoverLetter.Create(content).Value;

        Assert.True(coverLetter1.Equals(coverLetter2));
    }

    [Fact]
    public void Equals_WithDifferentContent_ShouldReturnFalse()
    {
        var coverLetter1 = CoverLetter.Create("Content 1").Value;
        var coverLetter2 = CoverLetter.Create("Content 2").Value;

        Assert.False(coverLetter1.Equals(coverLetter2));
    }
}
