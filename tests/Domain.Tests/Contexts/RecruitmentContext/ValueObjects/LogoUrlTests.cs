using Domain.Contexts.ApplicationContext.ValueObjects;
using Domain.Contexts.RecruitmentContext.ValueObjects;

namespace Domain.Tests.Contexts.RecruitmentContext.ValueObjects;

public class LogoUrlTests
{
    [Theory]
    [MemberData(nameof(ValidLogoUrls))]
    public void Create_ValidLogoUrl_ShouldReturnSuccess(string validUrl)
    {
        var result = LogoUrl.Create(validUrl);

        Assert.True(result.IsSuccess);
        Assert.Equal(validUrl, result.Value.Value);
    }

    [Theory]
    [MemberData(nameof(InvalidLogoUrls))]
    public void Create_InvalidLogoUrl_ShouldReturnFailure(string url)
    {
        var result = LogoUrl.Create(url);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Create_WhenExceedingMaxLength_ShouldReturnFailure()
    {
        var longUrl = "https://" + new string('a', LogoUrl.MaxLength) + ".com/file.pdf";

        var result = LogoUrl.Create(longUrl);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Theory]
    [MemberData(nameof(ValidLogoUrls))]
    public void Equals_SameLogoUrl_ShouldReturnTrue(string url)
    {
        var logoUrl1 = LogoUrl.Create(url).Value;
        var logoUrl2 = LogoUrl.Create(url).Value;

        Assert.True(logoUrl1.Equals(logoUrl2));
    }

    [Fact]
    public void Equals_DifferentLogoUrl_ShouldReturnFalse()
    {
        var logoUrl1 = LogoUrl.Create("https://example.com/logo1.png").Value;
        var logoUrl2 = LogoUrl.Create("https://example.com/logo2.png").Value;

        Assert.False(logoUrl1.Equals(logoUrl2));
    }

    [Fact]
    public void None_ShouldReturnLogoUrlWithEmptyValue()
    {
        var none = LogoUrl.None;

        Assert.Equal(string.Empty, none.Value);
    }

    public static TheoryData<string> ValidLogoUrls => new()
    {
        "https://example.com/logo.png",
        "http://assets.company.org/images/logo.jpg",
        "https://cdn.example.net/logos/company-logo.svg",
        "https://www.storage.com/uploads/brand.gif",
        "http://media.example.co.uk/logo.webp"
    };

    public static TheoryData<string> InvalidLogoUrls => new()
    {
        "ftp://example.com/logo.png",
        "example.com/logo.png",
        "https://",
        "https://.com/logo.png",
        "https://example/logo.png",
        "https://example..com/logo.png",
        "https://example .com/logo.png",
        "not a url",
        string.Empty,
        null!,
        "   ",
        "://example.com/logo.png"
    };
}