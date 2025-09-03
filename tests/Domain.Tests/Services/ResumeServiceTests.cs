using Domain.Abstraction.Interfaces;
using Domain.Contexts.IdentityContext.Aggregates;
using Domain.Contexts.IdentityContext.Enums;
using Domain.Contexts.JobPostingContext.ValueObjects;
using Domain.Contexts.ResumePostingContext.Enums;
using Domain.Contexts.ResumePostingContext.ValueObjects;
using Domain.Services;
using Domain.Shared.ValueObjects;
using Moq;

namespace Domain.Tests.Services;

public class ResumeServiceTests
{
    private readonly Mock<IMarkdownParser> _markdownParserMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private User _validJobSeekerUser;
    private User _validEmployerUser;
    private User _userWithoutAccount;
    private PersonalInfo _validPersonalInfo;
    private Location _validLocation;
    private ContactInfo _validContactInfo;
    private DesiredPosition _validDesiredPosition;
    private Money _validSalary;
    private RichTextContent _validSkillsDescription;
    private List<EmploymentType> _validEmploymentTypes;
    private List<WorkArrangement> _validWorkArrangements;

    public ResumeServiceTests()
    {
        _markdownParserMock = new Mock<IMarkdownParser>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
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
    }

    private void SetupTestData()
    {
        var jobSeekerEmail = Email.Create("jobseeker@example.com").Value;
        var jobSeekerPhoneNumber = PhoneNumber.Create("+14155552671", "US").Value;
        _validJobSeekerUser = User.Create("John", "Doe", UserRole.JobSeeker, jobSeekerEmail, jobSeekerPhoneNumber).Value;
        _validJobSeekerUser.CreateAccount("password123", _passwordHasherMock.Object);

        var employerEmail = Email.Create("employer@example.com").Value;
        var employerPhoneNumber = PhoneNumber.Create("+14155552672", "US").Value;
        _validEmployerUser = User.Create("Jane", "Smith", UserRole.Employer, employerEmail, employerPhoneNumber).Value;
        _validEmployerUser.CreateAccount("password123", _passwordHasherMock.Object);

        var userWithoutAccountEmail = Email.Create("noAccount@example.com").Value;
        var userWithoutAccountNumber = PhoneNumber.Create("+14155552673", "US").Value;
        _userWithoutAccount = User.Create("Bob", "Wilson", UserRole.JobSeeker, userWithoutAccountEmail, userWithoutAccountNumber).Value;

        _validPersonalInfo = PersonalInfo.Create("John", "Doe").Value;
        _validLocation = Location.Create("USA", "New York").Value;
        _validContactInfo = ContactInfo.Create(
            Email.Create("john.doe@example.com").Value,
            PhoneNumber.Create("+14156667777", "US").Value).Value;
        _validDesiredPosition = DesiredPosition.Create("Software Engineer").Value;
        _validSalary = Money.Create(75000m, "USD").Value;
        _validSkillsDescription = RichTextContent.Create("C#, .NET, React", _markdownParserMock.Object).Value;
        _validEmploymentTypes = [EmploymentType.FullTime, EmploymentType.Contract];
        _validWorkArrangements = [WorkArrangement.Remote, WorkArrangement.Hybrid];
    }

    [Fact]
    public void CreateResume_WithValidJobSeekerAndAccount_ShouldReturnSuccess()
    {
        var result = ResumeService.CreateResume(
            _validJobSeekerUser,
            _validPersonalInfo,
            _validLocation,
            _validContactInfo,
            _validDesiredPosition,
            _validSalary,
            _validSkillsDescription,
            _validEmploymentTypes,
            _validWorkArrangements);

        Assert.True(result.IsSuccess);
        Assert.Equal(_validJobSeekerUser.Id, result.Value.SeekerId);
        Assert.Equal(_validPersonalInfo, result.Value.PersonalInfo);
        Assert.Equal(_validLocation, result.Value.Location);
        Assert.Equal(_validContactInfo, result.Value.ContactInfo);
        Assert.Equal(_validDesiredPosition, result.Value.DesiredPosition);
        Assert.Equal(_validSalary, result.Value.SalaryExpectation);
        Assert.Equal(_validSkillsDescription, result.Value.SkillsDescription);
        Assert.Equal(ResumeStatus.Draft, result.Value.Status);
        Assert.Equal(_validEmploymentTypes.Count, result.Value.EmploymentTypes.Count);
        Assert.Equal(_validWorkArrangements.Count, result.Value.WorkArrangements.Count);
        Assert.True(result.Value.CreatedAt <= DateTime.UtcNow);
        Assert.True(result.Value.LastUpdatedAt <= DateTime.UtcNow);
        Assert.Null(result.Value.PublishedAt);
        Assert.Equal(TimeSpan.Zero, result.Value.TotalExperience);
        Assert.Empty(result.Value.WorkExperiences);
        Assert.Empty(result.Value.Educations);
        Assert.Empty(result.Value.Languages);
    }

    [Fact]
    public void CreateResume_WithEmployerRole_ShouldReturnFailure()
    {
        var result = ResumeService.CreateResume(
            _validEmployerUser,
            _validPersonalInfo,
            _validLocation,
            _validContactInfo,
            _validDesiredPosition,
            _validSalary,
            _validSkillsDescription,
            _validEmploymentTypes,
            _validWorkArrangements);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void CreateResume_WithUserWithoutAccount_ShouldReturnFailure()
    {
        var result = ResumeService.CreateResume(
            _userWithoutAccount,
            _validPersonalInfo,
            _validLocation,
            _validContactInfo,
            _validDesiredPosition,
            _validSalary,
            _validSkillsDescription,
            _validEmploymentTypes,
            _validWorkArrangements);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void CreateResume_WithNullPersonalInfo_ShouldReturnFailure()
    {
        var result = ResumeService.CreateResume(
            _validJobSeekerUser,
            null,
            _validLocation,
            _validContactInfo,
            _validDesiredPosition,
            _validSalary,
            _validSkillsDescription,
            _validEmploymentTypes,
            _validWorkArrangements);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void CreateResume_WithNullLocation_ShouldReturnFailure()
    {
        var result = ResumeService.CreateResume(
            _validJobSeekerUser,
            _validPersonalInfo,
            null,
            _validContactInfo,
            _validDesiredPosition,
            _validSalary,
            _validSkillsDescription,
            _validEmploymentTypes,
            _validWorkArrangements);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void CreateResume_WithNullContactInfo_ShouldReturnFailure()
    {
        var result = ResumeService.CreateResume(
            _validJobSeekerUser,
            _validPersonalInfo,
            _validLocation,
            null,
            _validDesiredPosition,
            _validSalary,
            _validSkillsDescription,
            _validEmploymentTypes,
            _validWorkArrangements);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void CreateResume_WithNullDesiredPosition_ShouldReturnFailure()
    {
        var result = ResumeService.CreateResume(
            _validJobSeekerUser,
            _validPersonalInfo,
            _validLocation,
            _validContactInfo,
            null,
            _validSalary,
            _validSkillsDescription,
            _validEmploymentTypes,
            _validWorkArrangements);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void CreateResume_WithNullSalary_ShouldReturnFailure()
    {
        var result = ResumeService.CreateResume(
            _validJobSeekerUser,
            _validPersonalInfo,
            _validLocation,
            _validContactInfo,
            _validDesiredPosition,
            null,
            _validSkillsDescription,
            _validEmploymentTypes,
            _validWorkArrangements);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void CreateResume_WithNullSkillsDescription_ShouldReturnFailure()
    {
        var result = ResumeService.CreateResume(
            _validJobSeekerUser,
            _validPersonalInfo,
            _validLocation,
            _validContactInfo,
            _validDesiredPosition,
            _validSalary,
            null,
            _validEmploymentTypes,
            _validWorkArrangements);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void CreateResume_WithEmptyEmploymentTypes_ShouldReturnFailure()
    {
        var result = ResumeService.CreateResume(
            _validJobSeekerUser,
            _validPersonalInfo,
            _validLocation,
            _validContactInfo,
            _validDesiredPosition,
            _validSalary,
            _validSkillsDescription,
            new List<EmploymentType>(),
            _validWorkArrangements);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void CreateResume_WithEmptyWorkArrangements_ShouldReturnSuccess()
    {
        var result = ResumeService.CreateResume(
            _validJobSeekerUser,
            _validPersonalInfo,
            _validLocation,
            _validContactInfo,
            _validDesiredPosition,
            _validSalary,
            _validSkillsDescription,
            _validEmploymentTypes,
            new List<WorkArrangement>());

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }
}
