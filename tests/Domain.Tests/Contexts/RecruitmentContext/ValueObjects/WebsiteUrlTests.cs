using Domain.Contexts.ApplicationContext.ValueObjects;
using Domain.Contexts.RecruitmentContext.ValueObjects;

namespace Domain.Tests.Contexts.RecruitmentContext.ValueObjects;

public class WebsiteUrlTests
{
    [Theory]
    [MemberData(nameof(ValidWebsiteUrls))]
    public void Create_ValidWebsiteUrl_ShouldReturnSuccess(string validUrl)
    {
        var result = WebsiteUrl.Create(validUrl);

        Assert.True(result.IsSuccess);
        Assert.Equal(validUrl, result.Value.Value);
    }

    [Theory]
    [MemberData(nameof(InvalidWebsiteUrls))]
    public void Create_InvalidWebsiteUrl_ShouldReturnFailure(string url)
    {
        var result = WebsiteUrl.Create(url);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Create_WhenExceedingMaxLength_ShouldReturnFailure()
    {
        var longUrl = "https://" + new string('a', WebsiteUrl.MaxLength) + ".com/file.pdf";

        var result = WebsiteUrl.Create(longUrl);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Theory]
    [MemberData(nameof(ValidWebsiteUrls))]
    public void Equals_SameWebsiteUrl_ShouldReturnTrue(string url)
    {
        var websiteUrl1 = WebsiteUrl.Create(url).Value;
        var websiteUrl2 = WebsiteUrl.Create(url).Value;

        Assert.True(websiteUrl1.Equals(websiteUrl2));
    }

    [Fact]
    public void Equals_DifferentWebsiteUrl_ShouldReturnFalse()
    {
        var websiteUrl1 = WebsiteUrl.Create("https://company1.com").Value;
        var websiteUrl2 = WebsiteUrl.Create("https://company2.com").Value;

        Assert.False(websiteUrl1.Equals(websiteUrl2));
    }

    [Fact]
    public void None_ShouldReturnWebsiteUrlWithEmptyValue()
    {
        var none = WebsiteUrl.None;

        Assert.Equal(string.Empty, none.Value);
    }

    public static TheoryData<string> ValidWebsiteUrls => new()
    {
        "https://www.example.com",
        "http://company.org",
        "https://subdomain.example.co.uk",
        "https://example-company.com/about",
        "http://www.test-site.net/page?param=value"
    };

    public static TheoryData<string> InvalidWebsiteUrls => new()
    {
        "example.com",
        "https://",
        "https://.com",
        "https://example",
        "https://example..com",
        "https://example .com",
        "not a url",
        string.Empty,
        null!,
        "   ",
        "://example.com"
    };
}