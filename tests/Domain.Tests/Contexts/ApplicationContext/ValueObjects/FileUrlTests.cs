using Domain.Contexts.ApplicationContext.ValueObjects;

namespace Domain.Tests.Contexts.ApplicationContext.ValueObjects;

public class FileUrlTests
{
    [Theory]
    [MemberData(nameof(ValidFileUrls))]
    public void Create_ValidFileUrl_ShouldReturnSuccess(string validUrl)
    {
        var result = FileUrl.Create(validUrl);

        Assert.True(result.IsSuccess);
        Assert.Equal(validUrl, result.Value.Value);
    }

    [Theory]
    [MemberData(nameof(InvalidFileUrls))]
    public void Create_InvalidFileUrl_ShouldReturnFailure(string url)
    {
        var result = FileUrl.Create(url);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Theory]
    [MemberData(nameof(ValidFileUrls))]
    public void Equals_SameFileUrl_ShouldReturnTrue(string url)
    {
        var fileUrl1 = FileUrl.Create(url).Value;
        var fileUrl2 = FileUrl.Create(url).Value;

        Assert.True(fileUrl1.Equals(fileUrl2));
    }

    [Fact]
    public void Equals_DifferentFileUrl_ShouldReturnFalse()
    {
        var fileUrl1 = FileUrl.Create("https://example.com/file1.pdf").Value;
        var fileUrl2 = FileUrl.Create("https://example.com/file2.pdf").Value;

        Assert.False(fileUrl1.Equals(fileUrl2));
    }

    public static TheoryData<string> ValidFileUrls => new()
    {
        "https://example.com/document.pdf",
        "http://files.company.org/resume.docx",
        "https://storage.googleapis.com/bucket/file.txt",
        "https://www.dropbox.com/s/abc123/document.pdf",
        "http://cdn.example.net/files/image.jpg"
    };

    public static TheoryData<string> InvalidFileUrls => new()
    {
        "ftp://example.com/file.pdf",
        "example.com/file.pdf",
        "https://",
        "https://.com/file.pdf",
        "https://example/file.pdf",
        "https://example..com/file.pdf",
        "https://example .com/file.pdf",
        "not a url",
        string.Empty,
        null!,
        "   ",
        "://example.com/file.pdf"
    };
}