using Domain.Abstraction.Interfaces;
using Domain.Contexts.IdentityContext.Aggregates;
using Domain.Contexts.IdentityContext.Enums;
using Domain.Contexts.JobPostingContext.Aggregates;
using Domain.Contexts.JobPostingContext.Enums;
using Domain.Contexts.JobPostingContext.IDs;
using Domain.Contexts.JobPostingContext.ValueObjects;
using Domain.Contexts.RecruitmentContext.IDs;
using Domain.Repos.CompanyUsers;
using Domain.Repos.Users;
using Domain.Services;
using Domain.Shared.ValueObjects;
using Moq;

namespace Domain.Tests.Contexts.JobPostingContext.Aggregates;

public class VacancyTests
{
    private readonly Mock<IMarkdownParser> _markdownParserMock = new();
    private readonly VacancyTitle _validTitle;
    private readonly RichTextContent _validDescription;
    private readonly Salary _validSalary;
    private readonly CompanyId _validCompanyId;
    private readonly Location _validLocation;
    private readonly RecruiterInfo _validRecruiterInfo;
    private readonly UserService _userService;
    private User _validEmployerUser;
    private readonly Mock<ICompanyUserRepository> _companyUserRepositoryMock = new();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
    private Vacancy _validVacancy;
    private readonly VacancyService _vacancyService;
    private readonly Mock<IUserRepository> _userRepositoryMock = new();

    public VacancyTests()
    {
        _vacancyService = new VacancyService(_companyUserRepositoryMock.Object);
        _userService = new UserService(_userRepositoryMock.Object);

        _markdownParserMock.Setup(x => x.ToPlainText(It.IsAny<string>()))
            .Returns<string>(markdown =>
            {
                return markdown + " (plain text)";
            });

        _validTitle = VacancyTitle.Create("Software Engineer").Value;
        _validDescription = RichTextContent.Create("Job description", _markdownParserMock.Object).Value;
        _validSalary = Salary.Range(50000m, 80000m, "USD").Value;
        _validCompanyId = new CompanyId();
        _validLocation = Location.Create("USA", "New York").Value;
        _validRecruiterInfo = RecruiterInfo.Create(
            "John Doe",
            Email.Create("recruiter@company.com").Value,
            PhoneNumber.Create("+14156667777", "US").Value).Value;

        _userRepositoryMock.Setup(x => x.IsUniqueEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _userRepositoryMock.Setup(x => x.IsUniquePhoneNumberAsync(It.IsAny<PhoneNumber>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _passwordHasherMock.Setup(x => x.HashPassword(It.IsAny<string>()))
            .Returns<string>(p => p + "_hashed");

        CreateValidEmployerUserAsync();

        _companyUserRepositoryMock.Setup(repo => repo.GetCompanyIdByUserId(_validEmployerUser.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(_validCompanyId);

        SetupTestVacancy();
    }

    private async Task CreateValidEmployerUserAsync()
    {
        var employerEmail = Email.Create("employer@example.com").Value;
        var employerPhoneNumber = PhoneNumber.Create("+14156667777", "US").Value;
        _validEmployerUser = (await _userService.CreateUserAsync("Jane", "Smith", UserRole.CompanyEmployee, employerEmail, employerPhoneNumber, "employer_account", _passwordHasherMock.Object, CancellationToken.None)).Value;
    }

    private async Task SetupTestVacancy()
    {
        _validVacancy = (await _vacancyService.CreateVacancyInDraftStatusAsync(
            _validEmployerUser,
            _validTitle,
            _validDescription,
            _validSalary,
            _validLocation,
            _validRecruiterInfo,
            CancellationToken.None)).Value;
    }

    #region Update Title Tests

    [Fact]
    public void UpdateTitle_WithValidTitleInDraftStatus_ShouldReturnSuccess()
    {
        var newTitle = VacancyTitle.Create("Senior Software Engineer").Value;
        var originalLastUpdated = _validVacancy.LastUpdatedAt;

        var result = _validVacancy.UpdateTitle(newTitle);

        Assert.True(result.IsSuccess);
        Assert.Equal(newTitle, _validVacancy.Title);
        Assert.True(_validVacancy.LastUpdatedAt >= originalLastUpdated);
    }

    [Fact]
    public void UpdateTitle_WithNullTitle_ShouldReturnFailure()
    {
        var originalTitle = _validVacancy.Title;

        var result = _validVacancy.UpdateTitle(null!);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(originalTitle, _validVacancy.Title);
    }

    [Fact]
    public async Task UpdateTitle_WhenRegistered_ShouldReturnFailure()
    {
        var vacancy = await CreateRegisteredVacancy();
        var originalTitle = _validVacancy.Title;
        var newTitle = VacancyTitle.Create("Senior Software Engineer").Value;

        var result = vacancy.UpdateTitle(newTitle);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(originalTitle, vacancy.Title);
    }

    [Fact]
    public async Task UpdateTitle_WhenPublished_ShouldReturnSuccess()
    {
        var vacancy = await CreatePublishedVacancy();
        var newTitle = VacancyTitle.Create("Senior Software Engineer").Value;

        var result = vacancy.UpdateTitle(newTitle);

        Assert.True(result.IsSuccess);
        Assert.Equal(newTitle, vacancy.Title);
    }

    [Fact]
    public async Task UpdateTitle_WhenArchived_ShouldReturnFailure()
    {
        var vacancy = await CreateArchivedVacancy();
        var originalTitle = vacancy.Title;
        var newTitle = VacancyTitle.Create("Senior Software Engineer").Value;

        var result = vacancy.UpdateTitle(newTitle);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(originalTitle, vacancy.Title);
    }

    #endregion

    #region Update Description Tests

    [Fact]
    public void UpdateDescription_WithValidDescription_ShouldReturnSuccess()
    {
        var vacancy = _validVacancy;
        var newDescription = RichTextContent.Create("Updated job description", _markdownParserMock.Object).Value;

        var result = vacancy.UpdateDescripiton(newDescription);

        Assert.True(result.IsSuccess);
        Assert.Equal(newDescription, vacancy.Description);
    }

    [Fact]
    public void UpdateDescription_WithNullDescription_ShouldReturnFailure()
    {
        var vacancy = _validVacancy;
        var originalDescription = vacancy.Description;

        var result = vacancy.UpdateDescripiton(null!);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(originalDescription, vacancy.Description);
    }

    [Fact]
    public void UpdateDescription_WithEmptyDescription_ShouldReturnFailure()
    {
        var vacancy = _validVacancy;
        var originalDescription = vacancy.Description;

        var result = vacancy.UpdateDescripiton(RichTextContent.Create("", _markdownParserMock.Object).Value);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(originalDescription, vacancy.Description);
    }

    [Fact]
    public async Task UpdateDescription_WhenRegistered_ShouldReturnFailure()
    {
        var vacancy = await CreateRegisteredVacancy();
        var originalDescription = vacancy.Description;
        var newDescription = RichTextContent.Create("Updated job description", _markdownParserMock.Object).Value;

        var result = vacancy.UpdateDescripiton(newDescription);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(originalDescription, vacancy.Description);
    }

    [Fact]
    public async Task UpdateDescription_WhenPublished_ShouldReturnSuccess()
    {
        var vacancy = await CreatePublishedVacancy();
        var newDescription = RichTextContent.Create("Updated job description", _markdownParserMock.Object).Value;

        var result = vacancy.UpdateDescripiton(newDescription);

        Assert.True(result.IsSuccess);
        Assert.Equal(newDescription, vacancy.Description);
    }

    [Fact]
    public async Task UpdateDescription_WhenArchived_ShouldReturnFailure()
    {
        var vacancy = await CreateArchivedVacancy();
        var originalDescription = vacancy.Description;
        var newDescription = RichTextContent.Create("Updated job description", _markdownParserMock.Object).Value;

        var result = vacancy.UpdateDescripiton(newDescription);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(originalDescription, vacancy.Description);
    }

    #endregion

    #region Update Salary Tests

    [Fact]
    public void UpdateSalary_WithValidSalary_ShouldReturnSuccess()
    {
        var vacancy = _validVacancy;
        var newSalary = Salary.Range(60000m, 90000m, "USD").Value;

        var result = vacancy.UpdateSalary(newSalary);

        Assert.True(result.IsSuccess);
        Assert.Equal(newSalary, vacancy.Salary);
    }

    [Fact]
    public void UpdateSalary_WithNullSalary_ShouldReturnFailure()
    {
        var vacancy = _validVacancy;
        var originalSalary = vacancy.Salary;

        var result = vacancy.UpdateSalary(null!);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(originalSalary, vacancy.Salary);
    }

    [Fact]
    public async Task UpdateSalary_WhenRegistered_ShouldReturnFailure()
    {
        var vacancy = await CreateRegisteredVacancy();
        var originalSalary = vacancy.Salary;
        var newSalary = Salary.Range(60000m, 90000m, "USD").Value;

        var result = vacancy.UpdateSalary(newSalary);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(originalSalary, vacancy.Salary);
    }

    [Fact]
    public async Task UpdateSalary_WhenPublished_ShouldReturnSuccess()
    {
        var vacancy = await CreatePublishedVacancy();
        var newSalary = Salary.Range(60000m, 90000m, "USD").Value;

        var result = vacancy.UpdateSalary(newSalary);

        Assert.True(result.IsSuccess);
        Assert.Equal(newSalary, vacancy.Salary);
    }

    [Fact]
    public async Task UpdateSalary_WhenArchived_ShouldReturnFailure()
    {
        var vacancy = await CreateArchivedVacancy();
        var originalSalary = vacancy.Salary;
        var newSalary = Salary.Range(60000m, 90000m, "USD").Value;

        var result = vacancy.UpdateSalary(newSalary);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(originalSalary, vacancy.Salary);
    }

    #endregion

    #region Update Location Tests

    [Fact]
    public void UpdateLocation_WithValidLocation_ShouldReturnSuccess()
    {
        var vacancy = _validVacancy;
        var newLocation = Location.Create("Canada", "Toronto").Value;

        var result = vacancy.UpdateLocation(newLocation);

        Assert.True(result.IsSuccess);
        Assert.Equal(newLocation, vacancy.Location);
    }

    [Fact]
    public void UpdateLocation_WithNullLocation_ShouldReturnFailure()
    {
        var vacancy = _validVacancy;
        var originalLocation = vacancy.Location;

        var result = vacancy.UpdateLocation(null!);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(originalLocation, vacancy.Location);
    }

    [Fact]
    public async Task UpdateLocation_WhenRegistered_ShouldReturnFailure()
    {
        var vacancy = await CreateRegisteredVacancy();
        var originalLocation = vacancy.Location;
        var newLocation = Location.Create("Canada", "Toronto").Value;

        var result = vacancy.UpdateLocation(newLocation);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(originalLocation, vacancy.Location);
    }

    [Fact]
    public async Task UpdateLocation_WhenPublished_ShouldReturnSuccess()
    {
        var vacancy = await CreatePublishedVacancy();
        var newLocation = Location.Create("Canada", "Toronto").Value;

        var result = vacancy.UpdateLocation(newLocation);

        Assert.True(result.IsSuccess);
        Assert.Equal(newLocation, vacancy.Location);
    }

    [Fact]
    public async Task UpdateLocation_WhenArchived_ShouldReturnFailure()
    {
        var vacancy = await CreateArchivedVacancy();
        var originalLocation = vacancy.Location;
        var newLocation = Location.Create("Canada", "Toronto").Value;

        var result = vacancy.UpdateLocation(newLocation);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(originalLocation, vacancy.Location);
    }

    #endregion

    #region Update RecruiterInfo Tests

    [Fact]
    public void UpdateRecruiterInfo_WithValidRecruiterInfo_ShouldReturnSuccess()
    {
        var vacancy = _validVacancy;
        var newRecruiterInfo = RecruiterInfo.Create(
            "Jane Smith",
            Email.Create("newrecruiter@company.com").Value,
            PhoneNumber.Create("+442034567890", "GB").Value).Value;

        var result = vacancy.UpdateRecruiterInfo(newRecruiterInfo);

        Assert.True(result.IsSuccess);
        Assert.Equal(newRecruiterInfo, vacancy.RecruiterInfo);
    }

    [Fact]
    public void UpdateRecruiterInfo_WithNullRecruiterInfo_ShouldReturnFailure()
    {
        var vacancy = _validVacancy;
        var originalRecruiterInfo = vacancy.RecruiterInfo;

        var result = vacancy.UpdateRecruiterInfo(null!);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(originalRecruiterInfo, vacancy.RecruiterInfo);
    }

    [Fact]
    public async Task UpdateRecruiterInfo_WhenRegistered_ShouldReturnFailure()
    {
        var vacancy = await CreateRegisteredVacancy();
        var originalRecruiterInfo = vacancy.RecruiterInfo;
        var newRecruiterInfo = RecruiterInfo.Create(
            "Test Recruiter",
            Email.Create("test@example.com").Value,
            PhoneNumber.Create("+14156667777", "US").Value).Value;

        var result = vacancy.UpdateRecruiterInfo(newRecruiterInfo);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(originalRecruiterInfo, vacancy.RecruiterInfo);
    }

    [Fact]
    public async Task UpdateRecruiterInfo_WhenPublished_ShouldReturnSuccess()
    {
        var vacancy = await CreatePublishedVacancy();
        var newRecruiterInfo = RecruiterInfo.Create(
            "Test Recruiter",
            Email.Create("test@example.com").Value,
            PhoneNumber.Create("+14156667777", "US").Value).Value;

        var result = vacancy.UpdateRecruiterInfo(newRecruiterInfo);

        Assert.True(result.IsSuccess);
        Assert.Equal(newRecruiterInfo, vacancy.RecruiterInfo);
    }

    [Fact]
    public async Task UpdateRecruiterInfo_WhenArchived_ShouldReturnFailure()
    {
        var vacancy = await CreateArchivedVacancy();
        var originalRecruiterInfo = vacancy.RecruiterInfo;
        var newRecruiterInfo = RecruiterInfo.Create(
            "Test Recruiter",
            Email.Create("test@example.com").Value,
            PhoneNumber.Create("+14156667777", "US").Value).Value;

        var result = vacancy.UpdateRecruiterInfo(newRecruiterInfo);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(originalRecruiterInfo, vacancy.RecruiterInfo);
    }

    #endregion

    #region UpdateCategoryId Tests

    [Fact]
    public async Task UpdateCategoryId_InRegisteredStatus_ShouldReturnSuccess()
    {
        var vacancy = await CreateRegisteredVacancy();
        var categoryId = new CategoryId();

        var result = vacancy.UpdateCategoryId(categoryId);

        Assert.True(result.IsSuccess);
        Assert.Equal(categoryId, vacancy.CategoryId);
    }

    [Fact]
    public void UpdateCategoryId_InDraftStatus_ShouldReturnFailure()
    {
        var vacancy = _validVacancy;
        var categoryId = new CategoryId();

        var result = vacancy.UpdateCategoryId(categoryId);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public async Task UpdateCategoryId_InPublishedStatus_ShouldReturnFailure()
    {
        var vacancy = await CreatePublishedVacancy();
        var categoryId = new CategoryId();

        var result = vacancy.UpdateCategoryId(categoryId);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public async Task UpdateCategoryId_InArchivedStatus_ShouldReturnFailure()
    {
        var vacancy = await CreateArchivedVacancy();
        var categoryId = new CategoryId();

        var result = vacancy.UpdateCategoryId(categoryId);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public async Task UpdateCategoryId_WithNullCategoryId_ShouldReturnFailure()
    {
        var vacancy = await CreateRegisteredVacancy();

        var result = vacancy.UpdateCategoryId(null!);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    #endregion

    #region Status Transition Tests

    [Fact]
    public void Register_FromDraftStatus_ShouldReturnSuccess()
    {
        var vacancy = _validVacancy;

        var result = vacancy.Register();

        Assert.True(result.IsSuccess);
        Assert.Equal(VacancyStatus.Registered, vacancy.Status);
        Assert.NotNull(vacancy.RegisteredAt);
        Assert.True(vacancy.RegisteredAt <= DateTime.UtcNow);
    }

    [Fact]
    public async Task Register_FromRegisteredStatus_ShouldReturnFailure()
    {
        var vacancy = await CreateRegisteredVacancy();

        var result = vacancy.Register();

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public async Task Publish_FromRegisteredStatusWithCategory_ShouldReturnSuccess()
    {
        var vacancy = await CreateRegisteredVacancy();
        var categoryId = new CategoryId();
        vacancy.UpdateCategoryId(categoryId);

        var result = vacancy.Publish();

        Assert.True(result.IsSuccess);
        Assert.Equal(VacancyStatus.Published, vacancy.Status);
        Assert.NotNull(vacancy.PublishedAt);
        Assert.True(vacancy.PublishedAt <= DateTime.UtcNow);
    }

    [Fact]
    public async Task Publish_FromRegisteredStatusWithoutCategory_ShouldReturnFailure()
    {
        var vacancy = await CreateRegisteredVacancy();

        var result = vacancy.Publish();

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Publish_FromDraftStatus_ShouldReturnFailure()
    {
        var vacancy = _validVacancy;

        var result = vacancy.Publish();

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public async Task Archive_FromPublishedStatus_ShouldReturnSuccess()
    {
        var vacancy = await CreatePublishedVacancy();

        var result = vacancy.Archive();

        Assert.True(result.IsSuccess);
        Assert.Equal(VacancyStatus.Archived, vacancy.Status);
    }

    [Fact]
    public void Archive_FromDraftStatus_ShouldReturnFailure()
    {
        var vacancy = _validVacancy;

        var result = vacancy.Archive();

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public async Task Archive_FromRegisteredStatus_ShouldReturnFailure()
    {
        var vacancy = await CreateRegisteredVacancy();

        var result = vacancy.Archive();

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    #endregion

    #region Full Workflow Tests

    [Fact]
    public void FullWorkflow_DraftToPublished_ShouldWork()
    {
        Assert.Equal(VacancyStatus.Draft, _validVacancy.Status);

        var registerResult = _validVacancy.Register();
        Assert.True(registerResult.IsSuccess);
        Assert.Equal(VacancyStatus.Registered, _validVacancy.Status);
        Assert.NotNull(_validVacancy.RegisteredAt);

        var categoryId = new CategoryId();
        var categoryResult = _validVacancy.UpdateCategoryId(categoryId);
        Assert.True(categoryResult.IsSuccess);
        Assert.Equal(categoryId, _validVacancy.CategoryId);

        var publishResult = _validVacancy.Publish();
        Assert.True(publishResult.IsSuccess);
        Assert.Equal(VacancyStatus.Published, _validVacancy.Status);
        Assert.NotNull(_validVacancy.PublishedAt);

        var archiveResult = _validVacancy.Archive();
        Assert.True(archiveResult.IsSuccess);
        Assert.Equal(VacancyStatus.Archived, _validVacancy.Status);
    }

    [Fact]
    public async Task FullWorkflow_ArchivedToPublished_ShouldWork()
    {
        var vacancy = await CreateArchivedVacancy();

        var publishResult = vacancy.Publish();

        Assert.True(publishResult.IsSuccess);
        Assert.Equal(VacancyStatus.Published, vacancy.Status);
        Assert.NotNull(vacancy.PublishedAt);
    }

    #endregion

    #region Helper Methods

    private async Task<Vacancy> CreateRegisteredVacancy()
    {
        var vacancy = (await _vacancyService.CreateVacancyInRegisteredStatusAsync(
            _validEmployerUser,
            _validTitle,
            _validDescription,
            _validSalary,
            _validLocation,
            _validRecruiterInfo,
            CancellationToken.None)).Value;
        return vacancy;
    }

    private async Task<Vacancy> CreatePublishedVacancy()
    {
        var vacancy = await CreateRegisteredVacancy();
        var categoryId = new CategoryId();
        vacancy.UpdateCategoryId(categoryId);
        vacancy.Publish();
        return vacancy;
    }

    private async Task<Vacancy> CreateArchivedVacancy()
    {
        var vacancy = await CreatePublishedVacancy();
        vacancy.Archive();
        return vacancy;
    }

    #endregion
}
