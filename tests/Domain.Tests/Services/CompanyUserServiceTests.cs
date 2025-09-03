using Domain.Abstraction.Interfaces;
using Domain.Contexts.IdentityContext.Aggregates;
using Domain.Contexts.IdentityContext.Enums;
using Domain.Contexts.RecruitmentContext.Aggregates;
using Domain.Contexts.RecruitmentContext.ValueObjects;
using Domain.Repos;
using Domain.Services;
using Domain.Shared.ValueObjects;
using Moq;

namespace Domain.Tests.Services;

public class CompanyUserServiceTests
{
    private readonly Mock<ICompanyUserRepository> _companyUserRepositoryMock;
    private readonly Mock<IMarkdownParser> _markdownParserMock;
    private readonly CompanyUserService _companyUserService;
    private User _validEmployerUser;
    private User _validJobSeekerUser;
    private Company _validCompany;

    public CompanyUserServiceTests()
    {
        _companyUserRepositoryMock = new Mock<ICompanyUserRepository>();
        _markdownParserMock = new Mock<IMarkdownParser>();
        _companyUserService = new CompanyUserService(_companyUserRepositoryMock.Object);

        SetupMocks();
        SetupTestData();
    }

    private void SetupMocks()
    {
        _markdownParserMock.Setup(x => x.ToPlainText(It.IsAny<string>()))
                          .Returns<string>(markdown => markdown.Replace("**", "").Replace("*", ""));

        _companyUserRepositoryMock.Setup(x => x.IsAlreadyAssignedToCompanyAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(false);

        _companyUserRepositoryMock.Setup(x => x.IsAlreadyAssignedAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(false);
    }

    private void SetupTestData()
    {
        var email = Email.Create("employer@example.com").Value;
        var phoneNumber = PhoneNumber.Create("+14156667777", "US").Value;
        _validEmployerUser = User.Create("John", "Doe", UserRole.Employer, email, phoneNumber).Value;

        var jobSeekerEmail = Email.Create("jobseeker@example.com").Value;
        var jobSeekerPhoneNumber = PhoneNumber.Create("+14158889999", "US").Value;
        _validJobSeekerUser = User.Create("Jane", "Smith", UserRole.JobSeeker, jobSeekerEmail, jobSeekerPhoneNumber).Value;

        var description = RichTextContent.Create("Company description", _markdownParserMock.Object).Value;
        var websiteUrl = WebsiteUrl.Create("https://example.com").Value;
        var logoUrl = LogoUrl.Create("https://example.com/logo.png").Value;
        _validCompany = Company.Create("Example Company", description, websiteUrl, logoUrl, 100).Value;
    }

    #region AssignEmployerToCompanyAsync Tests

    [Fact]
    public async Task AssignEmployerToCompanyAsync_WithValidEmployerAndCompany_ShouldReturnSuccess()
    {
        var result = await _companyUserService.AssignEmployerToCompanyAsync(_validEmployerUser, _validCompany, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(_validEmployerUser.Id, result.Value.RecruiterId);
        Assert.Equal(_validCompany.Id, result.Value.CompanyId);
        Assert.NotNull(result.Value.Id);

        _companyUserRepositoryMock.Verify(x => x.IsAlreadyAssignedToCompanyAsync(_validEmployerUser.Id.Value, It.IsAny<CancellationToken>()), Times.Once);
        _companyUserRepositoryMock.Verify(x => x.IsAlreadyAssignedAsync(_validEmployerUser.Id.Value, _validCompany.Id.Value, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AssignEmployerToCompanyAsync_WithJobSeekerRole_ShouldReturnFailure()
    {
        var result = await _companyUserService.AssignEmployerToCompanyAsync(_validJobSeekerUser, _validCompany, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);

        _companyUserRepositoryMock.Verify(x => x.IsAlreadyAssignedToCompanyAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _companyUserRepositoryMock.Verify(x => x.IsAlreadyAssignedAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AssignEmployerToCompanyAsync_WhenUserAlreadyAssignedToAnyCompany_ShouldReturnFailure()
    {
        _companyUserRepositoryMock.Setup(x => x.IsAlreadyAssignedToCompanyAsync(_validEmployerUser.Id.Value, It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(true);

        var result = await _companyUserService.AssignEmployerToCompanyAsync(_validEmployerUser, _validCompany, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);

        _companyUserRepositoryMock.Verify(x => x.IsAlreadyAssignedToCompanyAsync(_validEmployerUser.Id.Value, It.IsAny<CancellationToken>()), Times.Once);
        _companyUserRepositoryMock.Verify(x => x.IsAlreadyAssignedAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AssignEmployerToCompanyAsync_WhenUserAlreadyAssignedToSameCompany_ShouldReturnFailure()
    {
        _companyUserRepositoryMock.Setup(x => x.IsAlreadyAssignedAsync(_validEmployerUser.Id.Value, _validCompany.Id.Value, It.IsAny<CancellationToken>()))
                                 .ReturnsAsync(true);

        var result = await _companyUserService.AssignEmployerToCompanyAsync(_validEmployerUser, _validCompany, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);

        _companyUserRepositoryMock.Verify(x => x.IsAlreadyAssignedToCompanyAsync(_validEmployerUser.Id.Value, It.IsAny<CancellationToken>()), Times.Once);
        _companyUserRepositoryMock.Verify(x => x.IsAlreadyAssignedAsync(_validEmployerUser.Id.Value, _validCompany.Id.Value, It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion
}
