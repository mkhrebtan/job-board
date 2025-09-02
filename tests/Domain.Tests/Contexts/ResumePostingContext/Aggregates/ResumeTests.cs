using Domain.Abstraction.Interfaces;
using Domain.Contexts.IdentityContext.Aggregates;
using Domain.Contexts.IdentityContext.Enums;
using Domain.Contexts.ResumePostingContext.Aggregates;
using Domain.Contexts.ResumePostingContext.Entities;
using Domain.Contexts.ResumePostingContext.Enums;
using Domain.Contexts.ResumePostingContext.IDs;
using Domain.Contexts.ResumePostingContext.ValueObjects;
using Domain.Contexts.JobPostingContext.ValueObjects;
using Domain.Services;
using Domain.Shared.ValueObjects;
using Moq;

namespace Domain.Tests.Contexts.ResumePostingContext.Aggregates;

public class ResumeTests
{
    private readonly Mock<IMarkdownParser> _markdownParserMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private User _validJobSeekerUser;
    private User _validEmployerUser;
    private PersonalInfo _validPersonalInfo;
    private Location _validLocation;
    private ContactInfo _validContactInfo;
    private DesiredPosition _validDesiredPosition;
    private Money _validSalary;
    private RichTextContent _validSkillsDescription;
    private List<EmploymentType> _validEmploymentTypes;
    private List<WorkArrangement> _validWorkArrangements;

    public ResumeTests()
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
        _validJobSeekerUser = User.Create("John", "Doe", UserRole.JobSeeker, jobSeekerEmail).Value;
        _validJobSeekerUser.CreateAccount("password123", _passwordHasherMock.Object);

        var employerEmail = Email.Create("employer@example.com").Value;
        _validEmployerUser = User.Create("Jane", "Smith", UserRole.Employer, employerEmail).Value;
        _validEmployerUser.CreateAccount("password123", _passwordHasherMock.Object);

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

    #region Update Personal Info Tests

    [Fact]
    public void UpdatePersonalInfo_WithValidInfo_ShouldReturnSuccess()
    {
        var resume = CreateValidResume();
        var newPersonalInfo = PersonalInfo.Create("Jane", "Smith").Value;

        var result = resume.UpdatePersonalInfo(newPersonalInfo);

        Assert.True(result.IsSuccess);
        Assert.Equal(newPersonalInfo, resume.PersonalInfo);
    }

    [Fact]
    public void UpdatePersonalInfo_WithNull_ShouldReturnFailure()
    {
        var resume = CreateValidResume();
        var originalPersonalInfo = resume.PersonalInfo;

        var result = resume.UpdatePersonalInfo(null!);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(originalPersonalInfo, resume.PersonalInfo);
    }

    #endregion

    #region Update Location Tests

    [Fact]
    public void UpdateLocation_WithValidLocation_ShouldReturnSuccess()
    {
        var resume = CreateValidResume();
        var newLocation = Location.Create("Canada", "Toronto").Value;

        var result = resume.UpdateLocation(newLocation);

        Assert.True(result.IsSuccess);
        Assert.Equal(newLocation, resume.Location);
    }

    [Fact]
    public void UpdateLocation_WithNull_ShouldReturnFailure()
    {
        var resume = CreateValidResume();
        var originalLocation = resume.Location;

        var result = resume.UpdateLocation(null!);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(originalLocation, resume.Location);
    }

    #endregion

    #region Update Contact Info Tests

    [Fact]
    public void UpdateContactInfo_WithValidContactInfo_ShouldReturnSuccess()
    {
        var resume = CreateValidResume();
        var newContactInfo = ContactInfo.Create(
            Email.Create("newemail@example.com").Value,
            PhoneNumber.Create("+442034567890", "GB").Value).Value;

        var result = resume.UpdateContactInfo(newContactInfo);

        Assert.True(result.IsSuccess);
        Assert.Equal(newContactInfo, resume.ContactInfo);
    }

    [Fact]
    public void UpdateContactInfo_WithNull_ShouldReturnFailure()
    {
        var resume = CreateValidResume();
        var originalContactInfo = resume.ContactInfo;

        var result = resume.UpdateContactInfo(null!);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(originalContactInfo, resume.ContactInfo);
    }

    #endregion

    #region Update Desired Position Tests

    [Fact]
    public void UpdateDesiredPosition_WithValidPosition_ShouldReturnSuccess()
    {
        var resume = CreateValidResume();
        var newPosition = DesiredPosition.Create("Senior Software Engineer").Value;

        var result = resume.UpdateDesiredPosition(newPosition);

        Assert.True(result.IsSuccess);
        Assert.Equal(newPosition, resume.DesiredPosition);
    }

    [Fact]
    public void UpdateDesiredPosition_WithNull_ShouldReturnFailure()
    {
        var resume = CreateValidResume();
        var originalPosition = resume.DesiredPosition;

        var result = resume.UpdateDesiredPosition(null!);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(originalPosition, resume.DesiredPosition);
    }

    #endregion

    #region Update Salary Expectation Tests

    [Fact]
    public void UpdateSalaryExpectation_WithValidSalary_ShouldReturnSuccess()
    {
        var resume = CreateValidResume();
        var newSalary = Money.Create(95000m, "USD").Value;

        var result = resume.UpdateSalaryExpectation(newSalary);

        Assert.True(result.IsSuccess);
        Assert.Equal(newSalary, resume.SalaryExpectation);
    }

    [Fact]
    public void UpdateSalaryExpectation_WithNull_ShouldReturnFailure()
    {
        var resume = CreateValidResume();
        var originalSalary = resume.SalaryExpectation;

        var result = resume.UpdateSalaryExpectation(null!);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(originalSalary, resume.SalaryExpectation);
    }

    #endregion

    #region Update Skills Description Tests

    [Fact]
    public void UpdateSkillsDescription_WithValidDescription_ShouldReturnSuccess()
    {
        var resume = CreateValidResume();
        var newSkillsDescription = RichTextContent.Create("Updated skills", _markdownParserMock.Object).Value;

        var result = resume.UpdateSkillsDescription(newSkillsDescription);

        Assert.True(result.IsSuccess);
        Assert.Equal(newSkillsDescription, resume.SkillsDescription);
    }

    [Fact]
    public void UpdateSkillsDescription_WithNull_ShouldReturnFailure()
    {
        var resume = CreateValidResume();
        var originalDescription = resume.SkillsDescription;

        var result = resume.UpdateSkillsDescription(null!);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(originalDescription, resume.SkillsDescription);
    }

    #endregion

    #region Status Transition Tests

    [Fact]
    public void Publish_FromDraftStatus_ShouldReturnSuccess()
    {
        var resume = CreateValidResume();

        var result = resume.Publish();

        Assert.True(result.IsSuccess);
        Assert.Equal(ResumeStatus.Published, resume.Status);
        Assert.NotNull(resume.PublishedAt);
        Assert.True(resume.PublishedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Draft_FromPublishedStatus_ShouldReturnSuccess()
    {
        var resume = CreateValidResume();
        resume.Publish();

        var result = resume.Draft();

        Assert.True(result.IsSuccess);
        Assert.Equal(ResumeStatus.Draft, resume.Status);
    }

    #endregion

    #region Employment Type Tests

    [Fact]
    public void AddEmploymentType_WithValidType_ShouldReturnSuccess()
    {
        var resume = CreateValidResume();
        var newEmploymentType = EmploymentType.PartTime;

        var result = resume.AddEmploymentType(newEmploymentType);

        Assert.True(result.IsSuccess);
        Assert.Contains(newEmploymentType, resume.EmploymentTypes);
    }

    [Fact]
    public void AddEmploymentType_WithNull_ShouldReturnFailure()
    {
        var resume = CreateValidResume();

        var result = resume.AddEmploymentType(null);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void AddEmploymentType_WithDuplicate_ShouldReturnFailure()
    {
        var resume = CreateValidResume();
        var existingType = EmploymentType.FullTime;

        var result = resume.AddEmploymentType(existingType);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void RemoveEmploymentType_WithExistingType_ShouldReturnSuccess()
    {
        var resume = CreateValidResume();
        var typeToRemove = EmploymentType.FullTime;

        var result = resume.RemoveEmploymentType(typeToRemove);

        Assert.True(result.IsSuccess);
        Assert.DoesNotContain(typeToRemove, resume.EmploymentTypes);
    }

    [Fact]
    public void RemoveEmploymentType_WithNonExistingType_ShouldReturnFailure()
    {
        var resume = CreateValidResume();
        var nonExistingType = EmploymentType.Internship;

        var result = resume.RemoveEmploymentType(nonExistingType);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void RemoveEmploymentType_WithNull_ShouldReturnFailure()
    {
        var resume = CreateValidResume();

        var result = resume.RemoveEmploymentType(null);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    #endregion

    #region Work Arrangement Tests

    [Fact]
    public void AddWorkArrangement_WithValidArrangement_ShouldReturnSuccess()
    {
        var resume = CreateValidResume();
        var newWorkArrangement = WorkArrangement.OnSite;

        var result = resume.AddWorkArrangement(newWorkArrangement);

        Assert.True(result.IsSuccess);
        Assert.Contains(newWorkArrangement, resume.WorkArrangements);
    }

    [Fact]
    public void AddWorkArrangement_WithNull_ShouldReturnFailure()
    {
        var resume = CreateValidResume();

        var result = resume.AddWorkArrangement(null);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void AddWorkArrangement_WithDuplicate_ShouldReturnFailure()
    {
        var resume = CreateValidResume();
        var existingArrangement = WorkArrangement.Remote;

        var result = resume.AddWorkArrangement(existingArrangement);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void RemoveWorkArrangement_WithExistingArrangement_ShouldReturnSuccess()
    {
        var resume = CreateValidResume();
        var arrangementToRemove = WorkArrangement.Remote;

        var result = resume.RemoveWorkArrangement(arrangementToRemove);

        Assert.True(result.IsSuccess);
        Assert.DoesNotContain(arrangementToRemove, resume.WorkArrangements);
    }

    [Fact]
    public void RemoveWorkArrangement_WithNonExistingArrangement_ShouldReturnFailure()
    {
        var resume = CreateValidResume();
        var nonExistingArrangement = WorkArrangement.Shift;

        var result = resume.RemoveWorkArrangement(nonExistingArrangement);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    #endregion

    #region Work Experience Tests

    [Fact]
    public void AddWorkExperience_WithValidData_ShouldReturnSuccess()
    {
        var resume = CreateValidResume();
        var dateRange = DateRange.Create(new DateTime(2020, 1, 1), new DateTime(2023, 12, 31)).Value;
        var description = RichTextContent.Create("Software development", _markdownParserMock.Object).Value;

        var result = resume.AddWorkExperience("Tech Company", "Software Engineer", dateRange, description);

        Assert.True(result.IsSuccess);
        Assert.Single(resume.WorkExperiences);
        Assert.True(resume.TotalExperience > TimeSpan.Zero);
    }

    [Fact]
    public void RemoveWorkExperience_WithExistingId_ShouldReturnSuccess()
    {
        var resume = CreateValidResume();
        var dateRange = DateRange.Create(new DateTime(2020, 1, 1), new DateTime(2023, 12, 31)).Value;
        var description = RichTextContent.Create("Software development", _markdownParserMock.Object).Value;
        resume.AddWorkExperience("Tech Company", "Software Engineer", dateRange, description);
        var workExperienceId = resume.WorkExperiences.First().Id;

        var result = resume.RemoveWorkExperience(workExperienceId);

        Assert.True(result.IsSuccess);
        Assert.Empty(resume.WorkExperiences);
        Assert.Equal(TimeSpan.Zero, resume.TotalExperience);
    }

    [Fact]
    public void RemoveWorkExperience_WithNullId_ShouldReturnFailure()
    {
        var resume = CreateValidResume();

        var result = resume.RemoveWorkExperience(null);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void RemoveWorkExperience_WithNonExistingId_ShouldReturnFailure()
    {
        var resume = CreateValidResume();
        var nonExistingId = new WorkExperienceId();

        var result = resume.RemoveWorkExperience(nonExistingId);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    #endregion

    #region Education Tests

    [Fact]
    public void AddEducation_WithValidData_ShouldReturnSuccess()
    {
        var resume = CreateValidResume();
        var dateRange = DateRange.Create(new DateTime(2015, 9, 1), new DateTime(2019, 6, 30)).Value;

        var result = resume.AddEducation("University of Technology", "Bachelor of Science", "Computer Science", dateRange);

        Assert.True(result.IsSuccess);
        Assert.Single(resume.Educations);
    }

    [Fact]
    public void RemoveEducation_WithExistingId_ShouldReturnSuccess()
    {
        var resume = CreateValidResume();
        var dateRange = DateRange.Create(new DateTime(2015, 9, 1), new DateTime(2019, 6, 30)).Value;
        resume.AddEducation("University of Technology", "Bachelor of Science", "Computer Science", dateRange);
        var educationId = resume.Educations.First().Id;

        var result = resume.RemoveEducation(educationId);

        Assert.True(result.IsSuccess);
        Assert.Empty(resume.Educations);
    }

    [Fact]
    public void RemoveEducation_WithNullId_ShouldReturnFailure()
    {
        var resume = CreateValidResume();

        var result = resume.RemoveEducation(null);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void RemoveEducation_WithNonExistingId_ShouldReturnFailure()
    {
        var resume = CreateValidResume();
        var nonExistingId = new EducationId();

        var result = resume.RemoveEducation(nonExistingId);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    #endregion

    #region Language Tests

    [Fact]
    public void AddLanguage_WithValidLanguage_ShouldReturnSuccess()
    {
        var resume = CreateValidResume();

        var result = resume.AddLanguage("Spanish", LanguageLevel.B2);

        Assert.True(result.IsSuccess);
        Assert.Single(resume.Languages);
        Assert.Equal("Spanish", resume.Languages.First().Language);
        Assert.Equal(LanguageLevel.B2, resume.Languages.First().ProficiencyLevel);
    }

    [Fact]
    public void AddLanguage_WithDuplicateLanguage_ShouldReturnFailure()
    {
        var resume = CreateValidResume();
        resume.AddLanguage("English", LanguageLevel.C2);

        var result = resume.AddLanguage("english", LanguageLevel.B1);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void RemoveLanguage_WithExistingId_ShouldReturnSuccess()
    {
        var resume = CreateValidResume();
        resume.AddLanguage("French", LanguageLevel.A2);
        var languageId = resume.Languages.First().Id;

        var result = resume.RemoveLanguage(languageId);

        Assert.True(result.IsSuccess);
        Assert.Empty(resume.Languages);
    }

    [Fact]
    public void RemoveLanguage_WithNullId_ShouldReturnFailure()
    {
        var resume = CreateValidResume();

        var result = resume.RemoveLanguage(null);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void RemoveLanguage_WithNonExistingId_ShouldReturnFailure()
    {
        var resume = CreateValidResume();
        var nonExistingId = new LanguageId();

        var result = resume.RemoveLanguage(nonExistingId);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    #endregion

    #region TotalExperience Tests

    [Fact]
    public void TotalExperience_WithNoWorkExperience_ShouldReturnZero()
    {
        var resume = CreateValidResume();

        Assert.Equal(TimeSpan.Zero, resume.TotalExperience);
    }

    [Fact]
    public void TotalExperience_WithMultipleWorkExperiences_ShouldReturnCorrectSum()
    {
        var resume = CreateValidResume();
        var dateRange1 = DateRange.Create(new DateTime(2020, 1, 1), new DateTime(2022, 12, 31)).Value;
        var dateRange2 = DateRange.Create(new DateTime(2023, 1, 1), new DateTime(2023, 12, 31)).Value;
        var description = RichTextContent.Create("Work description", _markdownParserMock.Object).Value;
        var totalDays = dateRange1.Duration.TotalDays + dateRange2.Duration.TotalDays;
        var expectedTotalExperience = TimeSpan.FromDays(totalDays);

        resume.AddWorkExperience("Company 1", "Developer", dateRange1, description);
        resume.AddWorkExperience("Company 2", "Senior Developer", dateRange2, description);

        Assert.Equal(expectedTotalExperience, resume.TotalExperience);
    }

    #endregion

    #region Complete Workflow Tests

    [Fact]
    public void CompleteResumeWorkflow_WithResumeService_ShouldWork()
    {
        var resume = ResumeService.CreateResume(
            _validJobSeekerUser,
            _validPersonalInfo,
            _validLocation,
            _validContactInfo,
            _validDesiredPosition,
            _validSalary,
            _validSkillsDescription,
            _validEmploymentTypes,
            _validWorkArrangements).Value;
        
        Assert.Equal(ResumeStatus.Draft, resume.Status);

        var workDateRange = DateRange.Create(new DateTime(2020, 1, 1), new DateTime(2023, 12, 31)).Value;
        var workDescription = RichTextContent.Create("Software development work", _markdownParserMock.Object).Value;
        var workResult = resume.AddWorkExperience("Tech Corp", "Software Engineer", workDateRange, workDescription);
        Assert.True(workResult.IsSuccess);

        var eduDateRange = DateRange.Create(new DateTime(2015, 9, 1), new DateTime(2019, 6, 30)).Value;
        var eduResult = resume.AddEducation("Tech University", "Bachelor of Science", "Computer Science", eduDateRange);
        Assert.True(eduResult.IsSuccess);

        var langResult1 = resume.AddLanguage("English", LanguageLevel.C2);
        var langResult2 = resume.AddLanguage("Spanish", LanguageLevel.B1);
        Assert.True(langResult1.IsSuccess);
        Assert.True(langResult2.IsSuccess);

        var empTypeResult = resume.AddEmploymentType(EmploymentType.PartTime);
        var workArrResult = resume.AddWorkArrangement(WorkArrangement.OnSite);
        Assert.True(empTypeResult.IsSuccess);
        Assert.True(workArrResult.IsSuccess);

        var publishResult = resume.Publish();
        Assert.True(publishResult.IsSuccess);
        Assert.Equal(ResumeStatus.Published, resume.Status);

        Assert.Single(resume.WorkExperiences);
        Assert.Single(resume.Educations);
        Assert.Equal(2, resume.Languages.Count);
        Assert.Equal(3, resume.EmploymentTypes.Count);
        Assert.Equal(3, resume.WorkArrangements.Count);
        Assert.True(resume.TotalExperience > TimeSpan.Zero);
        Assert.NotNull(resume.PublishedAt);
    }

    #endregion

    #region Helper Methods

    private Resume CreateValidResume()
    {
        return ResumeService.CreateResume(
            _validJobSeekerUser,
            _validPersonalInfo,
            _validLocation,
            _validContactInfo,
            _validDesiredPosition,
            _validSalary,
            _validSkillsDescription,
            _validEmploymentTypes,
            _validWorkArrangements).Value;
    }

    #endregion
}
