using Domain.Abstraction.Interfaces;
using Domain.Contexts.IdentityContext.Aggregates;
using Domain.Contexts.IdentityContext.Enums;
using Domain.Contexts.JobPostingContext.Enums;
using Domain.Contexts.JobPostingContext.ValueObjects;
using Domain.Contexts.RecruitmentContext.IDs;
using Domain.Repos.CompanyUsers;
using Domain.Repos.Users;
using Domain.Services;
using Domain.Shared.ValueObjects;
using Moq;

namespace Domain.Tests.Services;

public class VacancyServiceTests
{
    private readonly VacancyService _vacancyService;
    private readonly Mock<ICompanyUserRepository> _companyUserRepositoryMock = new();
    private readonly Mock<IMarkdownParser> _markdownParserMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
    private readonly UserService _userService;
    private User _validEmployerUser;
    private readonly VacancyTitle _validTitle;
    private readonly RichTextContent _validDescription;
    private readonly Salary _validSalary;
    private readonly CompanyId _validCompanyId;
    private readonly Location _validLocation;
    private readonly RecruiterInfo _validRecruiterInfo;

    public VacancyServiceTests()
    {
        _vacancyService = new VacancyService(_companyUserRepositoryMock.Object);
        _userService = new UserService(_userRepositoryMock.Object);

        _userRepositoryMock.Setup(x => x.IsUniqueEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _userRepositoryMock.Setup(x => x.IsUniquePhoneNumberAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        SetupTestEmployer();

        _validTitle = VacancyTitle.Create("Software Engineer").Value;
        _validDescription = RichTextContent.Create("Job description", _markdownParserMock.Object).Value;
        _validSalary = Salary.Range(50000m, 80000m, "USD").Value;
        _validCompanyId = new CompanyId();
        _validLocation = Location.Create("USA", "New York").Value;
        _validRecruiterInfo = RecruiterInfo.Create(
            "John Doe",
            Email.Create("recruiter@company.com").Value,
            PhoneNumber.Create("+14156667777", "US").Value).Value;
        SetupMocks();
    }

    private void SetupMocks()
    {
        _markdownParserMock.Setup(x => x.ToPlainText(It.IsAny<string>()))
            .Returns<string>(markdown =>
            {
                return markdown + " (plain text)";
            });

        _companyUserRepositoryMock.Setup(repo => repo.GetCompanyIdByUserId(_validEmployerUser.Id.Value, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_validCompanyId.Value);

        _passwordHasherMock.Setup(x => x.HashPassword(It.IsAny<string>()))
            .Returns<string>(p => p + "_hashed");
    }

    private async Task SetupTestEmployer()
    {
        var employerEmail = Email.Create("employer@example.com").Value;
        var employerPhoneNumber = PhoneNumber.Create("+14156667777", "US").Value;
        _validEmployerUser = (await _userService.CreateUserAsync("Jane", "Smith", UserRole.Employer, employerEmail, employerPhoneNumber, CancellationToken.None)).Value;
        _validEmployerUser.CreateAccount("employer_account", _passwordHasherMock.Object);
    }

    #region CreateDraft Tests

    [Fact]
    public async Task CreateVacancyInDraftStatusAsync_WithValidInputs_ShouldReturnSuccess()
    {
        var result = await _vacancyService.CreateVacancyInDraftStatusAsync(
            _validEmployerUser,
            _validTitle,
            _validDescription,
            _validSalary,
            _validLocation,
            _validRecruiterInfo,
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(_validTitle, result.Value.Title);
        Assert.Equal(_validDescription, result.Value.Description);
        Assert.Equal(VacancyStatus.Draft, result.Value.Status);
        Assert.Equal(_validSalary, result.Value.Salary);
        Assert.Equal(_validCompanyId, result.Value.CompanyId);
        Assert.Equal(_validLocation, result.Value.Location);
        Assert.Equal(_validRecruiterInfo, result.Value.RecruiterInfo);
        Assert.Null(result.Value.CategoryId);
        Assert.Null(result.Value.RegisteredAt);
        Assert.Null(result.Value.PublishedAt);
        _companyUserRepositoryMock.Verify(repo => repo.GetCompanyIdByUserId(_validEmployerUser.Id.Value, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateVacancyInDraftStatusAsync_WithNullTitle_ShouldReturnFailure()
    {
        var result = await _vacancyService.CreateVacancyInDraftStatusAsync(
            _validEmployerUser,
            null!,
            _validDescription,
            _validSalary,
            _validLocation,
            _validRecruiterInfo,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public async Task CreateVacancyInDraftStatusAsync_WithNullDescription_ShouldReturnFailure()
    {
        var result = await _vacancyService.CreateVacancyInDraftStatusAsync(
            _validEmployerUser,
            _validTitle,
            null!,
            _validSalary,
            _validLocation,
            _validRecruiterInfo,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public async Task CreateVacancyInDraftStatusAsync_WithNullSalary_ShouldReturnFailure()
    {
        var result = await _vacancyService.CreateVacancyInDraftStatusAsync(
            _validEmployerUser,
            _validTitle,
            _validDescription,
            null!,
            _validLocation,
            _validRecruiterInfo,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public async Task CreateVacancyInDraftStatusAsync_WithNullLocation_ShouldReturnFailure()
    {
        var result = await _vacancyService.CreateVacancyInDraftStatusAsync(
            _validEmployerUser,
            _validTitle,
            _validDescription,
            _validSalary,
            null!,
            _validRecruiterInfo,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public async Task CreateVacancyInDraftStatusAsync_WithNullRecruiterInfo_ShouldReturnFailure()
    {
        var result = await _vacancyService.CreateVacancyInDraftStatusAsync(
            _validEmployerUser,
            _validTitle,
            _validDescription,
            _validSalary,
            _validLocation,
            null!,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public async Task CreateVacancyInDraftStatusAsync_WithNotEmployerUser_ShouldReturnFailure()
    {
        var invalidUser = await CreateNotEmployerUser();

        var result = await _vacancyService.CreateVacancyInDraftStatusAsync(
            invalidUser,
            _validTitle,
            _validDescription,
            _validSalary,
            _validLocation,
            _validRecruiterInfo,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        _companyUserRepositoryMock.Verify(repo => repo.GetCompanyIdByUserId(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateVacancyInDraftStatusAsync_WithEmployerWithoutAccount_ShouldReturnFailure()
    {
        var invalidUser = await CreateEmployerUserWithoutAccount();

        var result = await _vacancyService.CreateVacancyInDraftStatusAsync(
            invalidUser,
            _validTitle,
            _validDescription,
            _validSalary,
            _validLocation,
            _validRecruiterInfo,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        _companyUserRepositoryMock.Verify(repo => repo.GetCompanyIdByUserId(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateVacancyInDraftStatusAsync_WhenUseHasNoCompany_ShouldReturnFailure()
    {
        _companyUserRepositoryMock.Setup(repo => repo.GetCompanyIdByUserId(_validEmployerUser.Id.Value, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid?)null);

        var result = await _vacancyService.CreateVacancyInDraftStatusAsync(
            _validEmployerUser,
            _validTitle,
            _validDescription,
            _validSalary,
            _validLocation,
            _validRecruiterInfo,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        _companyUserRepositoryMock.Verify(repo => repo.GetCompanyIdByUserId(_validEmployerUser.Id.Value, It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region CreateAndRegister Tests

    [Fact]
    public async Task CreateVacancyInRegisteredStatusAsync_WithValidInputs_ShouldReturnSuccess()
    {
        var result = await _vacancyService.CreateVacancyInRegisteredStatusAsync(
            _validEmployerUser,
            _validTitle,
            _validDescription,
            _validSalary,
            _validLocation,
            _validRecruiterInfo,
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(_validTitle, result.Value.Title);
        Assert.Equal(_validDescription, result.Value.Description);
        Assert.Equal(VacancyStatus.Registered, result.Value.Status);
        Assert.Equal(_validSalary, result.Value.Salary);
        Assert.Equal(_validCompanyId, result.Value.CompanyId);
        Assert.Equal(_validLocation, result.Value.Location);
        Assert.Equal(_validRecruiterInfo, result.Value.RecruiterInfo);
        Assert.Null(result.Value.CategoryId);
        Assert.Null(result.Value.PublishedAt);
        _companyUserRepositoryMock.Verify(repo => repo.GetCompanyIdByUserId(_validEmployerUser.Id.Value, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateVacancyInRegisteredStatusAsync_WithNullTitle_ShouldReturnFailure()
    {
        var result = await _vacancyService.CreateVacancyInRegisteredStatusAsync(
            _validEmployerUser,
            null!,
            _validDescription,
            _validSalary,
            _validLocation,
            _validRecruiterInfo,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public async Task CreateVacancyInRegisteredStatusAsync_WithNullDescription_ShouldReturnFailure()
    {
        var result = await _vacancyService.CreateVacancyInRegisteredStatusAsync(
            _validEmployerUser,
            _validTitle,
            null!,
            _validSalary,
            _validLocation,
            _validRecruiterInfo,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public async Task CreateVacancyInRegisteredStatusAsync_WithNullSalary_ShouldReturnFailure()
    {
        var result = await _vacancyService.CreateVacancyInRegisteredStatusAsync(
            _validEmployerUser,
            _validTitle,
            _validDescription,
            null!,
            _validLocation,
            _validRecruiterInfo,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public async Task CreateVacancyInRegisteredStatusAsync_WithNullLocation_ShouldReturnFailure()
    {
        var result = await _vacancyService.CreateVacancyInRegisteredStatusAsync(
            _validEmployerUser,
            _validTitle,
            _validDescription,
            _validSalary,
            null!,
            _validRecruiterInfo,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public async Task CreateVacancyInRegisteredStatusAsync_WithNullRecruiterInfo_ShouldReturnFailure()
    {
        var result = await _vacancyService.CreateVacancyInRegisteredStatusAsync(
            _validEmployerUser,
            _validTitle,
            _validDescription,
            _validSalary,
            _validLocation,
            null!,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public async Task CreateVacancyInRegisteredStatusAsync_WithNotEmployerUser_ShouldReturnFailure()
    {
        var invalidUser = await CreateNotEmployerUser();

        var result = await _vacancyService.CreateVacancyInRegisteredStatusAsync(
            invalidUser,
            _validTitle,
            _validDescription,
            _validSalary,
            _validLocation,
            _validRecruiterInfo,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        _companyUserRepositoryMock.Verify(repo => repo.GetCompanyIdByUserId(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateVacancyInRegisteredStatusAsync_WithEmployerWithoutAccount_ShouldReturnFailure()
    {
        var invalidUser = await CreateEmployerUserWithoutAccount();

        var result = await _vacancyService.CreateVacancyInRegisteredStatusAsync(
            invalidUser,
            _validTitle,
            _validDescription,
            _validSalary,
            _validLocation,
            _validRecruiterInfo,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        _companyUserRepositoryMock.Verify(repo => repo.GetCompanyIdByUserId(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateVacancyInRegisteredStatusAsync_WhenUseHasNoCompany_ShouldReturnFailure()
    {
        _companyUserRepositoryMock.Setup(repo => repo.GetCompanyIdByUserId(_validEmployerUser.Id.Value, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid?)null);

        var result = await _vacancyService.CreateVacancyInRegisteredStatusAsync(
            _validEmployerUser,
            _validTitle,
            _validDescription,
            _validSalary,
            _validLocation,
            _validRecruiterInfo,
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        _companyUserRepositoryMock.Verify(repo => repo.GetCompanyIdByUserId(_validEmployerUser.Id.Value, It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    private async Task<User> CreateNotEmployerUser()
    {
        var userEmail = Email.Create("employer@example.com").Value;
        var userNumber = PhoneNumber.Create("+14156667777", "US").Value;
        return (await _userService.CreateUserAsync("Jane", "Smith", UserRole.JobSeeker, userEmail, userNumber, CancellationToken.None)).Value;
    }

    private async Task<User> CreateEmployerUserWithoutAccount()
    {
        var userEmail = Email.Create("employer@example.com").Value;
        var userNumber = PhoneNumber.Create("+14156667777", "US").Value;
        return (await _userService.CreateUserAsync("Jane", "Smith", UserRole.Employer, userEmail, userNumber, CancellationToken.None)).Value;
    }
}
