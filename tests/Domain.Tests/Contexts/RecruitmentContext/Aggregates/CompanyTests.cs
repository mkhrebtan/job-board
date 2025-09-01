using Domain.Abstraction.Interfaces;
using Domain.Contexts.RecruitmentContext.Aggregates;
using Domain.Contexts.RecruitmentContext.ValueObjects;
using Domain.Shared;
using Domain.Shared.ValueObjects;
using Moq;

namespace Domain.Tests.Contexts.RecruitmentContext.Aggregates;

public class CompanyTests
{
    private readonly Mock<IMarkdownParser> _markdownParser;
    private readonly RichTextContent _validDescription; 
    private readonly WebsiteUrl _validWebsiteUrl;
    private readonly LogoUrl _validLogoUrl;

    public CompanyTests()
    {
        _markdownParser = new Mock<IMarkdownParser>();
        SetupMarkdownParserMock();
        _validDescription = RichTextContent.Create("Test description", _markdownParser.Object).Value;
        _validWebsiteUrl = WebsiteUrl.Create("https://example.com").Value;
        _validLogoUrl = LogoUrl.Create("https://example.com/logo.png").Value;
    }

    private void SetupMarkdownParserMock()
    {
        _markdownParser.Setup(x => x.ToPlainText(It.IsAny<string>()))
            .Returns<string>(markdown =>
            {
                return markdown + " (plain text)";
            });
    }

    [Fact]
    public void Create_WithValidParameters_ShouldReturnSuccess()
    {
        var name = "Test Company";
        var size = 100;

        var result = Company.Create(name, _validDescription, _validWebsiteUrl, _validLogoUrl, size);

        Assert.True(result.IsSuccess);
        Assert.Equal(name, result.Value.Name);
        Assert.Equal(_validDescription, result.Value.Description);
        Assert.Equal(_validWebsiteUrl, result.Value.WebsiteUrl);
        Assert.Equal(_validLogoUrl, result.Value.LogoUrl);
        Assert.Equal(size, result.Value.Size);
        Assert.False(result.Value.IsVerified);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidName_ShouldReturnFailure(string invalidName)
    {
        var result = Company.Create(invalidName, _validDescription, _validWebsiteUrl, _validLogoUrl, 100);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Create_WithNullDescription_ShouldReturnFailure()
    {
        var result = Company.Create("Test Company", null, _validWebsiteUrl, _validLogoUrl, 100);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Create_WithNullWebsiteUrl_ShouldReturnFailure()
    {
        var result = Company.Create("Test Company", _validDescription, null, _validLogoUrl, 100);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Create_WithNullLogoUrl_ShouldReturnFailure()
    {
        var result = Company.Create("Test Company", _validDescription, _validWebsiteUrl, null, 100);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Create_WithInvalidSize_ShouldReturnFailure(int invalidSize)
    {
        var result = Company.Create("Test Company", _validDescription, _validWebsiteUrl, _validLogoUrl, invalidSize);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Create_WithNullSize_ShouldReturnSuccess()
    {
        var result = Company.Create("Test Company", _validDescription, _validWebsiteUrl, _validLogoUrl, null);

        Assert.True(result.IsSuccess);
        Assert.Null(result.Value.Size);
    }

    [Fact]
    public void Verify_WhenNotVerified_ShouldReturnSuccess()
    {
        var company = Company.Create("Test Company", _validDescription, _validWebsiteUrl, _validLogoUrl, 100).Value;

        var result = company.Verify();

        Assert.True(result.IsSuccess);
        Assert.True(company.IsVerified);
    }

    [Fact]
    public void Verify_WhenAlreadyVerified_ShouldReturnFailure()
    {
        var company = Company.Create("Test Company", _validDescription, _validWebsiteUrl, _validLogoUrl, 100).Value;
        company.Verify();

        var result = company.Verify();

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void UpdateName_WithValidName_ShouldReturnSuccess()
    {
        var company = Company.Create("Test Company", _validDescription, _validWebsiteUrl, _validLogoUrl, 100).Value;
        var newName = "Updated Company";

        var result = company.UpdateName(newName);

        Assert.True(result.IsSuccess);
        Assert.Equal(newName, company.Name);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void UpdateName_WithInvalidName_ShouldReturnFailure(string invalidName)
    {
        var company = Company.Create("Test Company", _validDescription, _validWebsiteUrl, _validLogoUrl, 100).Value;

        var result = company.UpdateName(invalidName);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void UpdateWebsiteUrl_WithValidUrl_ShouldReturnSuccess()
    {
        var company = Company.Create("Test Company", _validDescription, _validWebsiteUrl, _validLogoUrl, 100).Value;
        var newUrl = WebsiteUrl.Create("https://newsite.com").Value;

        var result = company.UpdateWebsiteUrl(newUrl);

        Assert.True(result.IsSuccess);
        Assert.Equal(newUrl, company.WebsiteUrl);
    }

    [Fact]
    public void UpdateWebsiteUrl_WithNull_ShouldReturnFailure()
    {
        var company = Company.Create("Test Company", _validDescription, _validWebsiteUrl, _validLogoUrl, 100).Value;

        var result = company.UpdateWebsiteUrl(null);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void UpdateLogoUrl_WithValidUrl_ShouldReturnSuccess()
    {
        var company = Company.Create("Test Company", _validDescription, _validWebsiteUrl, _validLogoUrl, 100).Value;
        var newLogoUrl = LogoUrl.Create("https://newsite.com/newlogo.png").Value;

        var result = company.UpdateLogoUrl(newLogoUrl);

        Assert.True(result.IsSuccess);
        Assert.Equal(newLogoUrl, company.LogoUrl);
    }

    [Fact]
    public void UpdateLogoUrl_WithNull_ShouldReturnFailure()
    {
        var company = Company.Create("Test Company", _validDescription, _validWebsiteUrl, _validLogoUrl, 100).Value;

        var result = company.UpdateLogoUrl(null);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void UpdateDescription_WithValidDescription_ShouldReturnSuccess()
    {
        var company = Company.Create("Test Company", _validDescription, _validWebsiteUrl, _validLogoUrl, 100).Value;
        var newDescription = RichTextContent.Create("Updated description", _markdownParser.Object).Value;

        var result = company.UpdateDescription(newDescription);

        Assert.True(result.IsSuccess);
        Assert.Equal(newDescription, company.Description);
    }

    [Fact]
    public void UpdateDescription_WithNull_ShouldReturnFailure()
    {
        var company = Company.Create("Test Company", _validDescription, _validWebsiteUrl, _validLogoUrl, 100).Value;

        var result = company.UpdateDescription(null);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void UpdateSize_WithValidSize_ShouldReturnSuccess()
    {
        var company = Company.Create("Test Company", _validDescription, _validWebsiteUrl, _validLogoUrl, 100).Value;
        var newSize = 200;

        var result = company.UpdateSize(newSize);

        Assert.True(result.IsSuccess);
        Assert.Equal(newSize, company.Size);
    }

    [Fact]
    public void UpdateSize_WithNull_ShouldReturnSuccess()
    {
        var company = Company.Create("Test Company", _validDescription, _validWebsiteUrl, _validLogoUrl, 100).Value;

        var result = company.UpdateSize(null);

        Assert.True(result.IsSuccess);
        Assert.Null(company.Size);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void UpdateSize_WithInvalidSize_ShouldReturnFailure(int invalidSize)
    {
        var company = Company.Create("Test Company", _validDescription, _validWebsiteUrl, _validLogoUrl, 100).Value;

        var result = company.UpdateSize(invalidSize);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }
}
