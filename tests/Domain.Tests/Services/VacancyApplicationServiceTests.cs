using Domain.Abstraction.Interfaces;
using Domain.Contexts.ApplicationContext.ValueObjects;
using Domain.Contexts.IdentityContext.Aggregates;
using Domain.Contexts.IdentityContext.Enums;
using Domain.Contexts.IdentityContext.IDs;
using Domain.Contexts.JobPostingContext.Aggregates;
using Domain.Contexts.JobPostingContext.Enums;
using Domain.Contexts.JobPostingContext.IDs;
using Domain.Contexts.JobPostingContext.ValueObjects;
using Domain.Contexts.RecruitmentContext.IDs;
using Domain.Contexts.ResumePostingContext.Aggregates;
using Domain.Contexts.ResumePostingContext.Enums;
using Domain.Contexts.ResumePostingContext.ValueObjects;
using Domain.Repos.CompanyUsers;
using Domain.Repos.Users;
using Domain.Repos.VacancyApplications;
using Domain.Services;
using Domain.Shared.ValueObjects;
using Moq;

namespace Domain.Tests.Services;

public class VacancyApplicationServiceTests
{
    private readonly Mock<IVacancyApplicationRepository> _vacancyApplicationRepositoryMock = new();
    private readonly Mock<IMarkdownParser> _markdownParserMock = new();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<ICompanyUserRepository> _companyUserRepositoryMock = new();
    private readonly UserService _userService;
    private readonly VacancyService _vacancyService;
    private readonly VacancyApplicationService _vacancyApplicationService;
    private User _validJobSeekerUser;
    private User _validEmployerUser;
    private Vacancy _validPublishedVacancy;
    private Vacancy _validDraftVacancy;
    private Resume _validResume;
    private CoverLetter _validCoverLetter;
    private FileUrl _validFileUrl;

    public VacancyApplicationServiceTests()
    {
        _vacancyApplicationService = new VacancyApplicationService(_vacancyApplicationRepositoryMock.Object);
        _userService = new UserService(_userRepositoryMock.Object);
        _vacancyService = new VacancyService(_companyUserRepositoryMock.Object);

        SetupMocks();
        SetupTestData();
    }

    private void SetupMocks()
    {
        _markdownParserMock.Setup(x => x.ToPlainText(It.IsAny<string>()))
                          .Returns<string>(markdown => markdown.Replace("**", "").Replace("*", ""));

        _passwordHasherMock.Setup(x => x.HashPassword(It.IsAny<string>()))
                          .Returns<string>(password => $"hashed_{password}");

        _passwordHasherMock.Setup(x => x.VerifyHashedPassword(It.IsAny<string>(), It.IsAny<string>()))
                          .Returns<string, string>((hashedPassword, providedPassword) =>
                              hashedPassword == $"hashed_{providedPassword}");

        _vacancyApplicationRepositoryMock.Setup(x => x.HasAlreadyAppliedToVacancyAsync(It.IsAny<UserId>(), It.IsAny<VacancyId>(), It.IsAny<CancellationToken>()))
                                        .ReturnsAsync(false);

        _userRepositoryMock.Setup(x => x.IsUniqueEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _userRepositoryMock.Setup(x => x.IsUniquePhoneNumberAsync(It.IsAny<PhoneNumber>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _companyUserRepositoryMock.Setup(x => x.GetCompanyIdByUserId(It.IsAny<UserId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CompanyId());
    }

    private async Task SetupTestData()
    {
        var jobSeekerEmail = Email.Create("jobseeker@example.com").Value;
        var jobSeekerPhoneNumber = PhoneNumber.Create("+14156667777", "US").Value;
        _validJobSeekerUser = (await _userService.CreateUserAsync("John", "Doe", UserRole.JobSeeker, jobSeekerEmail, jobSeekerPhoneNumber, "password123", _passwordHasherMock.Object, CancellationToken.None)).Value;

        var employerEmail = Email.Create("employer@example.com").Value;
        var employerPhoneNumber = PhoneNumber.Create("+14156667777", "US").Value;
        _validEmployerUser = (await _userService.CreateUserAsync("Jane", "Smith", UserRole.CompanyEmployee, employerEmail, employerPhoneNumber, "password123", _passwordHasherMock.Object, CancellationToken.None)).Value;

        var title = VacancyTitle.Create("Software Engineer").Value;
        var description = RichTextContent.Create("Job description", _markdownParserMock.Object).Value;
        var salary = Salary.Range(50000m, 80000m, "USD").Value;
        var location = Location.Create("USA", "New York").Value;
        var recruiterInfo = RecruiterInfo.Create(
            "John",
            Email.Create("recruiter@company.com").Value,
            PhoneNumber.Create("+14156667777", "US").Value).Value;

        _validPublishedVacancy = (await _vacancyService.CreateVacancyInRegisteredStatusAsync(
            _validEmployerUser, title, description, salary, location, recruiterInfo, CancellationToken.None)).Value;
        var categoryId = new CategoryId();
        _validPublishedVacancy.UpdateCategoryId(categoryId);
        _validPublishedVacancy.Publish();

        _validDraftVacancy = (await _vacancyService.CreateVacancyInDraftStatusAsync(
            _validEmployerUser, title, description, salary, location, recruiterInfo, CancellationToken.None)).Value;

        var personalInfo = PersonalInfo.Create("John", "Doe").Value;
        var contactInfo = ContactInfo.Create(
            Email.Create("john.doe@example.com").Value,
            PhoneNumber.Create("+14156667777", "US").Value).Value;
        var desiredPosition = DesiredPosition.Create("Software Engineer").Value;
        var salaryExpectation = Money.Create(75000m, "USD").Value;
        var skillsDescription = RichTextContent.Create("C#, .NET, React", _markdownParserMock.Object).Value;
        var employmentTypes = new List<EmploymentType> { EmploymentType.FullTime };
        var workArrangements = new List<WorkArrangement> { WorkArrangement.Remote };

        _validResume = ResumeService.CreateResume(
            _validJobSeekerUser,
            personalInfo,
            location,
            contactInfo,
            desiredPosition,
            salaryExpectation,
            skillsDescription,
            employmentTypes,
            workArrangements).Value;

        _validCoverLetter = CoverLetter.Create("Dear Hiring Manager, I am interested in this position.").Value;
        _validFileUrl = FileUrl.Create("https://example.com/resume.pdf").Value;
    }

    #region ApplyToVacancyWithCreatedResumeAsync Tests

    [Fact]
    public async Task ApplyToVacancyWithCreatedResumeAsync_WithValidInputs_ShouldReturnSuccess()
    {
        var result = await _vacancyApplicationService.ApplyToVacancyWithCreatedResumeAsync(
            _validJobSeekerUser,
            _validPublishedVacancy,
            _validCoverLetter,
            _validResume);

        Assert.True(result.IsSuccess);
        Assert.Equal(_validJobSeekerUser.Id, result.Value.SeekerId);
        Assert.Equal(_validPublishedVacancy.Id, result.Value.VacancyId);
        Assert.Equal(_validCoverLetter, result.Value.CoverLetter);
        Assert.Equal(_validResume.Id, result.Value.ResumeId);

        _vacancyApplicationRepositoryMock.Verify(x => x.HasAlreadyAppliedToVacancyAsync(
            _validJobSeekerUser.Id,
            _validPublishedVacancy.Id,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ApplyToVacancyWithCreatedResumeAsync_WithEmployerRole_ShouldReturnFailure()
    {
        var result = await _vacancyApplicationService.ApplyToVacancyWithCreatedResumeAsync(
            _validEmployerUser,
            _validPublishedVacancy,
            _validCoverLetter,
            _validResume);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);

        _vacancyApplicationRepositoryMock.Verify(x => x.HasAlreadyAppliedToVacancyAsync(
            It.IsAny<UserId>(), It.IsAny<VacancyId>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ApplyToVacancyWithCreatedResumeAsync_WithDraftVacancy_ShouldReturnFailure()
    {
        var result = await _vacancyApplicationService.ApplyToVacancyWithCreatedResumeAsync(
            _validJobSeekerUser,
            _validDraftVacancy,
            _validCoverLetter,
            _validResume);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);

        _vacancyApplicationRepositoryMock.Verify(x => x.HasAlreadyAppliedToVacancyAsync(
            It.IsAny<UserId>(), It.IsAny<VacancyId>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ApplyToVacancyWithCreatedResumeAsync_WhenUserAlreadyApplied_ShouldReturnFailure()
    {
        _vacancyApplicationRepositoryMock.Setup(x => x.HasAlreadyAppliedToVacancyAsync(
            _validJobSeekerUser.Id,
            _validPublishedVacancy.Id,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _vacancyApplicationService.ApplyToVacancyWithCreatedResumeAsync(
            _validJobSeekerUser,
            _validPublishedVacancy,
            _validCoverLetter,
            _validResume);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);

        _vacancyApplicationRepositoryMock.Verify(x => x.HasAlreadyAppliedToVacancyAsync(
            _validJobSeekerUser.Id,
            _validPublishedVacancy.Id,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region ApplyToVacancyWithFileAsync Tests

    [Fact]
    public async Task ApplyToVacancyWithFileAsync_WithValidInputs_ShouldReturnSuccess()
    {
        var result = await _vacancyApplicationService.ApplyToVacancyWithFileAsync(
            _validJobSeekerUser,
            _validPublishedVacancy,
            _validCoverLetter,
            _validFileUrl);

        Assert.True(result.IsSuccess);
        Assert.Equal(_validJobSeekerUser.Id, result.Value.SeekerId);
        Assert.Equal(_validPublishedVacancy.Id, result.Value.VacancyId);
        Assert.Equal(_validCoverLetter, result.Value.CoverLetter);
        Assert.Equal(_validFileUrl, result.Value.FileUrl);

        _vacancyApplicationRepositoryMock.Verify(x => x.HasAlreadyAppliedToVacancyAsync(
            _validJobSeekerUser.Id,
            _validPublishedVacancy.Id,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ApplyToVacancyWithFileAsync_WithEmployerRole_ShouldReturnFailure()
    {
        var result = await _vacancyApplicationService.ApplyToVacancyWithFileAsync(
            _validEmployerUser,
            _validPublishedVacancy,
            _validCoverLetter,
            _validFileUrl);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);

        _vacancyApplicationRepositoryMock.Verify(x => x.HasAlreadyAppliedToVacancyAsync(
            It.IsAny<UserId>(), It.IsAny<VacancyId>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ApplyToVacancyWithFileAsync_WithDraftVacancy_ShouldReturnFailure()
    {
        var result = await _vacancyApplicationService.ApplyToVacancyWithFileAsync(
            _validJobSeekerUser,
            _validDraftVacancy,
            _validCoverLetter,
            _validFileUrl);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);

        _vacancyApplicationRepositoryMock.Verify(x => x.HasAlreadyAppliedToVacancyAsync(
            It.IsAny<UserId>(), It.IsAny<VacancyId>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ApplyToVacancyWithFileAsync_WhenUserAlreadyApplied_ShouldReturnFailure()
    {
        _vacancyApplicationRepositoryMock.Setup(x => x.HasAlreadyAppliedToVacancyAsync(
            _validJobSeekerUser.Id,
            _validPublishedVacancy.Id,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _vacancyApplicationService.ApplyToVacancyWithFileAsync(
            _validJobSeekerUser,
            _validPublishedVacancy,
            _validCoverLetter,
            _validFileUrl);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);

        _vacancyApplicationRepositoryMock.Verify(x => x.HasAlreadyAppliedToVacancyAsync(
            _validJobSeekerUser.Id,
            _validPublishedVacancy.Id,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ApplyToVacancyWithFileAsync_WithNullFileUrl_ShouldReturnFailure()
    {
        var result = await _vacancyApplicationService.ApplyToVacancyWithFileAsync(
            _validJobSeekerUser,
            _validPublishedVacancy,
            _validCoverLetter,
            null!);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    #endregion

    #region Validation Logic Tests

    [Theory]
    [MemberData(nameof(NonPublishedStatuses))]
    public async Task ApplyToVacancyWithCreatedResumeAsync_WithNonPublishedVacancyStatus_ShouldReturnFailure(VacancyStatus status)
    {
        var vacancy = await CreateVacancyWithStatus(status);

        var result = await _vacancyApplicationService.ApplyToVacancyWithCreatedResumeAsync(
            _validJobSeekerUser,
            vacancy,
            _validCoverLetter,
            _validResume);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Theory]
    [MemberData(nameof(NonPublishedStatuses))]
    public async Task ApplyToVacancyWithFileAsync_WithNonPublishedVacancyStatus_ShouldReturnFailure(VacancyStatus status)
    {
        var vacancy = await CreateVacancyWithStatus(status);

        var result = await _vacancyApplicationService.ApplyToVacancyWithFileAsync(
            _validJobSeekerUser,
            vacancy,
            _validCoverLetter,
            _validFileUrl);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    #endregion

    #region Cover Letter Validation Tests

    [Fact]
    public async Task ApplyToVacancyWithCreatedResumeAsync_WithNullCoverLetter_ShouldReturnFailure()
    {
        var result = await _vacancyApplicationService.ApplyToVacancyWithCreatedResumeAsync(
            _validJobSeekerUser,
            _validPublishedVacancy,
            null,
            _validResume);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public async Task ApplyToVacancyWithFileAsync_WithNullCoverLetter_ShouldReturnFailure()
    {
        var result = await _vacancyApplicationService.ApplyToVacancyWithFileAsync(
            _validJobSeekerUser,
            _validPublishedVacancy,
            null,
            _validFileUrl);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    #endregion

    #region Concurrent Application Tests

    [Fact]
    public async Task MultipleApplicationAttempts_ShouldHandleRepositoryStateChanges()
    {
        var result1 = await _vacancyApplicationService.ApplyToVacancyWithCreatedResumeAsync(
            _validJobSeekerUser,
            _validPublishedVacancy,
            _validCoverLetter,
            _validResume);

        Assert.True(result1.IsSuccess);

        _vacancyApplicationRepositoryMock.Setup(x => x.HasAlreadyAppliedToVacancyAsync(
            _validJobSeekerUser.Id,
            _validPublishedVacancy.Id,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result2 = await _vacancyApplicationService.ApplyToVacancyWithFileAsync(
            _validJobSeekerUser,
            _validPublishedVacancy,
            _validCoverLetter,
            _validFileUrl);

        Assert.True(result2.IsFailure);
        Assert.NotNull(result2.Error);
    }

    #endregion

    #region Helper Methods

    private async Task<Vacancy> CreateVacancyWithStatus(VacancyStatus status)
    {
        var title = VacancyTitle.Create("Test Position").Value;
        var description = RichTextContent.Create("Test description", _markdownParserMock.Object).Value;
        var salary = Salary.Range(50000m, 80000m, "USD").Value;
        var location = Location.Create("USA", "Test City").Value;
        var recruiterInfo = RecruiterInfo.Create(
            "John",
            Email.Create("recruiter@test.com").Value,
            PhoneNumber.Create("+14156667777", "US").Value).Value;

        var employerEmail = Email.Create("test-employer@test.com").Value;
        var employerPhoneNumber = PhoneNumber.Create("+14156667778", "US").Value;
        var employerUser = (await _userService.CreateUserAsync("Test", "Employer", UserRole.CompanyEmployee, employerEmail, employerPhoneNumber, "password123", _passwordHasherMock.Object, CancellationToken.None)).Value;

        if (status == VacancyStatus.Draft)
        {
            return (await _vacancyService.CreateVacancyInDraftStatusAsync(
                employerUser, title, description, salary, location, recruiterInfo, CancellationToken.None)).Value;
        }

        if (status == VacancyStatus.Registered)
        {
            return (await _vacancyService.CreateVacancyInRegisteredStatusAsync(
                employerUser, title, description, salary, location, recruiterInfo, CancellationToken.None)).Value;
        }

        if (status == VacancyStatus.Published)
        {
            var vacancy = (await _vacancyService.CreateVacancyInRegisteredStatusAsync(
                employerUser, title, description, salary, location, recruiterInfo, CancellationToken.None)).Value;
            var categoryId = new CategoryId();
            vacancy.UpdateCategoryId(categoryId);
            vacancy.Publish();
            return vacancy;
        }

        if (status == VacancyStatus.Archived)
        {
            var vacancy = (await _vacancyService.CreateVacancyInRegisteredStatusAsync(
                employerUser, title, description, salary, location, recruiterInfo, CancellationToken.None)).Value;
            var categoryId = new CategoryId();
            vacancy.UpdateCategoryId(categoryId);
            vacancy.Publish();
            vacancy.Archive();
            return vacancy;
        }

        throw new ArgumentException($"Unsupported status: {status}");
    }

    #endregion

    public static readonly TheoryData<VacancyStatus> NonPublishedStatuses = new()
    {
        VacancyStatus.Draft,
        VacancyStatus.Registered,
        VacancyStatus.Archived
    };
}
