using Domain.Abstraction.Interfaces;
using Domain.Contexts.IdentityContext.IDs;
using Domain.Contexts.JobPostingContext.Aggregates;
using Domain.Contexts.JobPostingContext.Enums;
using Domain.Contexts.JobPostingContext.IDs;
using Domain.Contexts.JobPostingContext.ValueObjects;
using Domain.Contexts.RecruitmentContext.IDs;
using Domain.Shared.ValueObjects;
using Moq;

namespace Domain.Tests.Contexts.JobPostingContext.Aggregates;

public class VacancyTests
{
    private readonly Mock<IMarkdownParser> _markdownParserMock;
    private readonly VacancyTitle _validTitle;
    private readonly RichTextContent _validDescription;
    private readonly Salary _validSalary;
    private readonly CompanyId _validCompanyId;
    private readonly Location _validLocation;
    private readonly RecruiterInfo _validRecruiterInfo;

    public VacancyTests()
    {
        _markdownParserMock = new Mock<IMarkdownParser>();
        SetupMarkdownParserMock();

        _validTitle = VacancyTitle.Create("Software Engineer").Value;
        _validDescription = RichTextContent.Create("Job description", _markdownParserMock.Object).Value;
        _validSalary = Salary.Range(50000m, 80000m, "USD").Value;
        _validCompanyId = new CompanyId();
        _validLocation = Location.Create("USA", "New York").Value;
        _validRecruiterInfo = RecruiterInfo.Create(
            Email.Create("recruiter@company.com").Value,
            PhoneNumber.Create("+14156667777", "US").Value).Value;
    }

    private void SetupMarkdownParserMock()
    {
        _markdownParserMock.Setup(x => x.ToPlainText(It.IsAny<string>()))
            .Returns<string>(markdown =>
            {
                return markdown + " (plain text)";
            });
    }

    #region CreateDraft Tests

    [Fact]
    public void CreateDraft_WithValidInputs_ShouldReturnSuccess()
    {
        var result = Vacancy.CreateDraft(
            _validTitle,
            _validDescription,
            _validSalary,
            _validCompanyId,
            _validLocation,
            _validRecruiterInfo);

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
    }

    [Fact]
    public void CreateDraft_WithNullTitle_ShouldReturnFailure()
    {
        var result = Vacancy.CreateDraft(
            null,
            _validDescription,
            _validSalary,
            _validCompanyId,
            _validLocation,
            _validRecruiterInfo);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void CreateDraft_WithNullDescription_ShouldReturnFailure()
    {
        var result = Vacancy.CreateDraft(
            _validTitle,
            null,
            _validSalary,
            _validCompanyId,
            _validLocation,
            _validRecruiterInfo);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void CreateDraft_WithNullSalary_ShouldReturnFailure()
    {
        var result = Vacancy.CreateDraft(
            _validTitle,
            _validDescription,
            null,
            _validCompanyId,
            _validLocation,
            _validRecruiterInfo);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void CreateDraft_WithNullCompanyId_ShouldReturnFailure()
    {
        var result = Vacancy.CreateDraft(
            _validTitle,
            _validDescription,
            _validSalary,
            null,
            _validLocation,
            _validRecruiterInfo);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void CreateDraft_WithNullLocation_ShouldReturnFailure()
    {
        var result = Vacancy.CreateDraft(
            _validTitle,
            _validDescription,
            _validSalary,
            _validCompanyId,
            null,
            _validRecruiterInfo);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void CreateDraft_WithNullRecruiterInfo_ShouldReturnFailure()
    {
        var result = Vacancy.CreateDraft(
            _validTitle,
            _validDescription,
            _validSalary,
            _validCompanyId,
            _validLocation,
            null);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    #endregion

    #region CreateAndRegister Tests

    [Fact]
    public void CreateAndRegister_WithValidInputs_ShouldReturnSuccess()
    {
        var result = Vacancy.CreateAndRegister(
            _validTitle,
            _validDescription,
            _validSalary,
            _validCompanyId,
            _validLocation,
            _validRecruiterInfo);

        Assert.True(result.IsSuccess);
        Assert.Equal(VacancyStatus.Registered, result.Value.Status);
        Assert.Equal(_validTitle, result.Value.Title);
        Assert.Equal(_validDescription, result.Value.Description);
        Assert.Equal(_validSalary, result.Value.Salary);
        Assert.Equal(_validCompanyId, result.Value.CompanyId);
        Assert.Equal(_validLocation, result.Value.Location);
        Assert.Equal(_validRecruiterInfo, result.Value.RecruiterInfo);
        Assert.NotNull(result.Value.RegisteredAt);
        Assert.Null(result.Value.PublishedAt);
        Assert.Null(result.Value.CategoryId);
    }

    [Fact]
    public void CreateAndRegister_WithNullTitle_ShouldReturnFailure()
    {
        var result = Vacancy.CreateAndRegister(
            null,
            _validDescription,
            _validSalary,
            _validCompanyId,
            _validLocation,
            _validRecruiterInfo);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void CreateAndRegister_WithNullDescription_ShouldReturnFailure()
    {
        var result = Vacancy.CreateAndRegister(
            _validTitle,
            null,
            _validSalary,
            _validCompanyId,
            _validLocation,
            _validRecruiterInfo);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void CreateAndRegister_WithNullSalary_ShouldReturnFailure()
    {
        var result = Vacancy.CreateAndRegister(
            _validTitle,
            _validDescription,
            null,
            _validCompanyId,
            _validLocation,
            _validRecruiterInfo);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void CreateAndRegister_WithNullCompanyId_ShouldReturnFailure()
    {
        var result = Vacancy.CreateAndRegister(
            _validTitle,
            _validDescription,
            _validSalary,
            null,
            _validLocation,
            _validRecruiterInfo);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void CreateAndRegister_WithNullLocation_ShouldReturnFailure()
    {
        var result = Vacancy.CreateAndRegister(
            _validTitle,
            _validDescription,
            _validSalary,
            _validCompanyId,
            null,
            _validRecruiterInfo);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void CreateAndRegister_WithNullRecruiterInfo_ShouldReturnFailure()
    {
        var result = Vacancy.CreateAndRegister(
            _validTitle,
            _validDescription,
            _validSalary,
            _validCompanyId,
            _validLocation,
            null);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    #endregion

    #region Update Title Tests

    [Fact]
    public void UpdateTitle_WithValidTitleInDraftStatus_ShouldReturnSuccess()
    {
        var vacancy = Vacancy.CreateDraft(
            _validTitle,
            _validDescription,
            _validSalary,
            _validCompanyId,
            _validLocation,
            _validRecruiterInfo).Value;
        var newTitle = VacancyTitle.Create("Senior Software Engineer").Value;
        var originalLastUpdated = vacancy.LastUpdatedAt;

        var result = vacancy.UpdateTitle(newTitle);

        Assert.True(result.IsSuccess);
        Assert.Equal(newTitle, vacancy.Title);
        Assert.True(vacancy.LastUpdatedAt >= originalLastUpdated);
    }

    [Fact]
    public void UpdateTitle_WithNullTitle_ShouldReturnFailure()
    {
        var vacancy = Vacancy.CreateDraft(
            _validTitle,
            _validDescription,
            _validSalary,
            _validCompanyId,
            _validLocation,
            _validRecruiterInfo).Value;
        var originalTitle = vacancy.Title;

        var result = vacancy.UpdateTitle(null);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(originalTitle, vacancy.Title);
    }

    [Fact]
    public void UpdateTitle_WhenRegistered_ShouldReturnFailure()
    {
        var vacancy = Vacancy.CreateAndRegister(
            _validTitle,
            _validDescription,
            _validSalary,
            _validCompanyId,
            _validLocation,
            _validRecruiterInfo).Value;
        var originalTitle = vacancy.Title;
        var newTitle = VacancyTitle.Create("Senior Software Engineer").Value;

        var result = vacancy.UpdateTitle(newTitle);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(originalTitle, vacancy.Title);
    }

    [Fact]
    public void UpdateTitle_WhenPublished_ShouldReturnSuccess()
    {
        var vacancy = CreatePublishedVacancy();
        var newTitle = VacancyTitle.Create("Senior Software Engineer").Value;

        var result = vacancy.UpdateTitle(newTitle);

        Assert.True(result.IsSuccess);
        Assert.Equal(newTitle, vacancy.Title);
    }

    [Fact]
    public void UpdateTitle_WhenArchived_ShouldReturnFailure()
    {
        var vacancy = CreateArchivedVacancy();
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
        var vacancy = Vacancy.CreateDraft(
            _validTitle,
            _validDescription,
            _validSalary,
            _validCompanyId,
            _validLocation,
            _validRecruiterInfo).Value;
        var newDescription = RichTextContent.Create("Updated job description", _markdownParserMock.Object).Value;

        var result = vacancy.UpdateDescripiton(newDescription);

        Assert.True(result.IsSuccess);
        Assert.Equal(newDescription, vacancy.Description);
    }

    [Fact]
    public void UpdateDescription_WithNullDescription_ShouldReturnFailure()
    {
        var vacancy = Vacancy.CreateDraft(
            _validTitle,
            _validDescription,
            _validSalary,
            _validCompanyId,
            _validLocation,
            _validRecruiterInfo).Value;
        var originalDescription = vacancy.Description;

        var result = vacancy.UpdateDescripiton(null);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(originalDescription, vacancy.Description);
    }

    [Fact]
    public void UpdateDescription_WhenRegistered_ShouldReturnFailure()
    {
        var vacancy = Vacancy.CreateAndRegister(
            _validTitle,
            _validDescription,
            _validSalary,
            _validCompanyId,
            _validLocation,
            _validRecruiterInfo).Value;
        var originalDescription = vacancy.Description;
        var newDescription = RichTextContent.Create("Updated job description", _markdownParserMock.Object).Value;
        
        var result = vacancy.UpdateDescripiton(newDescription);
        
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(originalDescription, vacancy.Description);
    }

    [Fact]
    public void UpdateDescription_WhenPublished_ShouldReturnSuccess()
    {
        var vacancy = CreatePublishedVacancy();
        var newDescription = RichTextContent.Create("Updated job description", _markdownParserMock.Object).Value;

        var result = vacancy.UpdateDescripiton(newDescription);

        Assert.True(result.IsSuccess);
        Assert.Equal(newDescription, vacancy.Description);
    }

    [Fact]
    public void UpdateDescription_WhenArchived_ShouldReturnFailure()
    {
        var vacancy = CreateArchivedVacancy();
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
        var vacancy = Vacancy.CreateDraft(
            _validTitle,
            _validDescription,
            _validSalary,
            _validCompanyId,
            _validLocation,
            _validRecruiterInfo).Value;
        var newSalary = Salary.Range(60000m, 90000m, "USD").Value;

        var result = vacancy.UpdateSalary(newSalary);

        Assert.True(result.IsSuccess);
        Assert.Equal(newSalary, vacancy.Salary);
    }

    [Fact]
    public void UpdateSalary_WithNullSalary_ShouldReturnFailure()
    {
        var vacancy = Vacancy.CreateDraft(
            _validTitle,
            _validDescription,
            _validSalary,
            _validCompanyId,
            _validLocation,
            _validRecruiterInfo).Value;
        var originalSalary = vacancy.Salary;

        var result = vacancy.UpdateSalary(null);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(originalSalary, vacancy.Salary);
    }

    [Fact]
    public void UpdateSalary_WhenRegistered_ShouldReturnFailure()
    {
        var vacancy = Vacancy.CreateAndRegister(
            _validTitle,
            _validDescription,
            _validSalary,
            _validCompanyId,
            _validLocation,
            _validRecruiterInfo).Value;
        var originalSalary = vacancy.Salary;
        var newSalary = Salary.Range(60000m, 90000m, "USD").Value;

        var result = vacancy.UpdateSalary(newSalary);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(originalSalary, vacancy.Salary);
    }

    [Fact]
    public void UpdateSalary_WhenPublished_ShouldReturnSuccess()
    {
        var vacancy = CreatePublishedVacancy();
        var newSalary = Salary.Range(60000m, 90000m, "USD").Value;

        var result = vacancy.UpdateSalary(newSalary);

        Assert.True(result.IsSuccess);
        Assert.Equal(newSalary, vacancy.Salary);
    }

    [Fact]
    public void UpdateSalary_WhenArchived_ShouldReturnFailure()
    {
        var vacancy = CreateArchivedVacancy();
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
        var vacancy = Vacancy.CreateDraft(
            _validTitle,
            _validDescription,
            _validSalary,
            _validCompanyId,
            _validLocation,
            _validRecruiterInfo).Value;
        var newLocation = Location.Create("Canada", "Toronto").Value;

        var result = vacancy.UpdateLocation(newLocation);

        Assert.True(result.IsSuccess);
        Assert.Equal(newLocation, vacancy.Location);
    }

    [Fact]
    public void UpdateLocation_WithNullLocation_ShouldReturnFailure()
    {
        var vacancy = Vacancy.CreateDraft(
            _validTitle,
            _validDescription,
            _validSalary,
            _validCompanyId,
            _validLocation,
            _validRecruiterInfo).Value;
        var originalLocation = vacancy.Location;

        var result = vacancy.UpdateLocation(null);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(originalLocation, vacancy.Location);
    }

    [Fact]
    public void UpdateLocation_WhenRegistered_ShouldReturnFailure()
    {
        var vacancy = Vacancy.CreateAndRegister(
            _validTitle,
            _validDescription,
            _validSalary,
            _validCompanyId,
            _validLocation,
            _validRecruiterInfo).Value;
        var originalLocation = vacancy.Location;
        var newLocation = Location.Create("Canada", "Toronto").Value;

        var result = vacancy.UpdateLocation(newLocation);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(originalLocation, vacancy.Location);
    }

    [Fact]
    public void UpdateLocation_WhenPublished_ShouldReturnSuccess()
    {
        var vacancy = CreatePublishedVacancy();
        var newLocation = Location.Create("Canada", "Toronto").Value;

        var result = vacancy.UpdateLocation(newLocation);

        Assert.True(result.IsSuccess);
        Assert.Equal(newLocation, vacancy.Location);
    }

    [Fact]
    public void UpdateLocation_WhenArchived_ShouldReturnFailure()
    {
        var vacancy = CreateArchivedVacancy();
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
        var vacancy = Vacancy.CreateDraft(
            _validTitle,
            _validDescription,
            _validSalary,
            _validCompanyId,
            _validLocation,
            _validRecruiterInfo).Value;
        var newRecruiterInfo = RecruiterInfo.Create(
            Email.Create("newrecruiter@company.com").Value,
            PhoneNumber.Create("+442034567890", "GB").Value).Value;

        var result = vacancy.UpdateRecruiterInfo(newRecruiterInfo);

        Assert.True(result.IsSuccess);
        Assert.Equal(newRecruiterInfo, vacancy.RecruiterInfo);
    }

    [Fact]
    public void UpdateRecruiterInfo_WithNullRecruiterInfo_ShouldReturnFailure()
    {
        var vacancy = Vacancy.CreateDraft(
            _validTitle,
            _validDescription,
            _validSalary,
            _validCompanyId,
            _validLocation,
            _validRecruiterInfo).Value;
        var originalRecruiterInfo = vacancy.RecruiterInfo;

        var result = vacancy.UpdateRecruiterInfo(null);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(originalRecruiterInfo, vacancy.RecruiterInfo);
    }

    [Fact]
    public void UpdateRecruiterInfo_WhenRegistered_ShouldReturnFailure()
    {
        var vacancy = Vacancy.CreateAndRegister(
            _validTitle,
            _validDescription,
            _validSalary,
            _validCompanyId,
            _validLocation,
            _validRecruiterInfo).Value;
        var originalRecruiterInfo = vacancy.RecruiterInfo;
        var newRecruiterInfo = RecruiterInfo.Create(
            Email.Create("test@example.com").Value,
            PhoneNumber.Create("+14156667777", "US").Value).Value;

        var result = vacancy.UpdateRecruiterInfo(newRecruiterInfo);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(originalRecruiterInfo, vacancy.RecruiterInfo);
    }

    [Fact]
    public void UpdateRecruiterInfo_WhenPublished_ShouldReturnSuccess()
    {
        var vacancy = CreatePublishedVacancy();
        var originalRecruiterInfo = vacancy.RecruiterInfo;
        var newRecruiterInfo = RecruiterInfo.Create(
            Email.Create("test@example.com").Value,
            PhoneNumber.Create("+14156667777", "US").Value).Value;

        var result = vacancy.UpdateRecruiterInfo(newRecruiterInfo);

        Assert.True(result.IsSuccess);
        Assert.Equal(newRecruiterInfo, vacancy.RecruiterInfo);
    }

    [Fact]
    public void UpdateRecruiterInfo_WhenArchived_ShouldReturnFailure()
    {
        var vacancy = CreateArchivedVacancy();
        var originalRecruiterInfo = vacancy.RecruiterInfo;
        var newRecruiterInfo = RecruiterInfo.Create(
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
    public void UpdateCategoryId_InRegisteredStatus_ShouldReturnSuccess()
    {
        var vacancy = Vacancy.CreateAndRegister(
            _validTitle,
            _validDescription,
            _validSalary,
            _validCompanyId,
            _validLocation,
            _validRecruiterInfo).Value;
        var categoryId = new CategoryId();

        var result = vacancy.UpdateCategoryId(categoryId);

        Assert.True(result.IsSuccess);
        Assert.Equal(categoryId, vacancy.CategoryId);
    }

    [Fact]
    public void UpdateCategoryId_InDraftStatus_ShouldReturnFailure()
    {
        var vacancy = Vacancy.CreateDraft(
            _validTitle,
            _validDescription,
            _validSalary,
            _validCompanyId,
            _validLocation,
            _validRecruiterInfo).Value;
        var categoryId = new CategoryId();

        var result = vacancy.UpdateCategoryId(categoryId);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void UpdateCategoryId_InPublishedStatus_ShouldReturnFailure()
    {
        var vacancy = CreatePublishedVacancy();
        var categoryId = new CategoryId();

        var result = vacancy.UpdateCategoryId(categoryId);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void UpdateCategoryId_InArchivedStatus_ShouldReturnFailure()
    {
        var vacancy = CreateArchivedVacancy();
        var categoryId = new CategoryId();

        var result = vacancy.UpdateCategoryId(categoryId);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void UpdateCategoryId_WithNullCategoryId_ShouldReturnFailure()
    {
        var vacancy = Vacancy.CreateAndRegister(
            _validTitle,
            _validDescription,
            _validSalary,
            _validCompanyId,
            _validLocation,
            _validRecruiterInfo).Value;

        var result = vacancy.UpdateCategoryId(null);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    #endregion

    #region Status Transition Tests

    [Fact]
    public void Register_FromDraftStatus_ShouldReturnSuccess()
    {
        var vacancy = Vacancy.CreateDraft(
            _validTitle,
            _validDescription,
            _validSalary,
            _validCompanyId,
            _validLocation,
            _validRecruiterInfo).Value;

        var result = vacancy.Register();

        Assert.True(result.IsSuccess);
        Assert.Equal(VacancyStatus.Registered, vacancy.Status);
        Assert.NotNull(vacancy.RegisteredAt);
        Assert.True(vacancy.RegisteredAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Register_FromRegisteredStatus_ShouldReturnFailure()
    {
        var vacancy = Vacancy.CreateAndRegister(
            _validTitle,
            _validDescription,
            _validSalary,
            _validCompanyId,
            _validLocation,
            _validRecruiterInfo).Value;

        var result = vacancy.Register();

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Publish_FromRegisteredStatusWithCategory_ShouldReturnSuccess()
    {
        var vacancy = Vacancy.CreateAndRegister(
            _validTitle,
            _validDescription,
            _validSalary,
            _validCompanyId,
            _validLocation,
            _validRecruiterInfo).Value;
        var categoryId = new CategoryId();
        vacancy.UpdateCategoryId(categoryId);

        var result = vacancy.Publish();

        Assert.True(result.IsSuccess);
        Assert.Equal(VacancyStatus.Published, vacancy.Status);
        Assert.NotNull(vacancy.PublishedAt);
        Assert.True(vacancy.PublishedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Publish_FromRegisteredStatusWithoutCategory_ShouldReturnFailure()
    {
        var vacancy = Vacancy.CreateAndRegister(
            _validTitle,
            _validDescription,
            _validSalary,
            _validCompanyId,
            _validLocation,
            _validRecruiterInfo).Value;

        var result = vacancy.Publish();

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Publish_FromDraftStatus_ShouldReturnFailure()
    {
        var vacancy = Vacancy.CreateDraft(
            _validTitle,
            _validDescription,
            _validSalary,
            _validCompanyId,
            _validLocation,
            _validRecruiterInfo).Value;

        var result = vacancy.Publish();

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Archive_FromPublishedStatus_ShouldReturnSuccess()
    {
        var vacancy = CreatePublishedVacancy();

        var result = vacancy.Archive();

        Assert.True(result.IsSuccess);
        Assert.Equal(VacancyStatus.Archived, vacancy.Status);
    }

    [Fact]
    public void Archive_FromDraftStatus_ShouldReturnFailure()
    {
        var vacancy = Vacancy.CreateDraft(
            _validTitle,
            _validDescription,
            _validSalary,
            _validCompanyId,
            _validLocation,
            _validRecruiterInfo).Value;

        var result = vacancy.Archive();

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Archive_FromRegisteredStatus_ShouldReturnFailure()
    {
        var vacancy = Vacancy.CreateAndRegister(
            _validTitle,
            _validDescription,
            _validSalary,
            _validCompanyId,
            _validLocation,
            _validRecruiterInfo).Value;
        
        var result = vacancy.Archive();

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    #endregion

    #region Full Workflow Tests

    [Fact]
    public void FullWorkflow_DraftToPublished_ShouldWork()
    {
        var vacancy = Vacancy.CreateDraft(_validTitle, _validDescription, _validSalary, _validCompanyId, _validLocation, _validRecruiterInfo).Value;
        Assert.Equal(VacancyStatus.Draft, vacancy.Status);

        var registerResult = vacancy.Register();
        Assert.True(registerResult.IsSuccess);
        Assert.Equal(VacancyStatus.Registered, vacancy.Status);
        Assert.NotNull(vacancy.RegisteredAt);

        var categoryId = new CategoryId();
        var categoryResult = vacancy.UpdateCategoryId(categoryId);
        Assert.True(categoryResult.IsSuccess);
        Assert.Equal(categoryId, vacancy.CategoryId);

        var publishResult = vacancy.Publish();
        Assert.True(publishResult.IsSuccess);
        Assert.Equal(VacancyStatus.Published, vacancy.Status);
        Assert.NotNull(vacancy.PublishedAt);

        var archiveResult = vacancy.Archive();
        Assert.True(archiveResult.IsSuccess);
        Assert.Equal(VacancyStatus.Archived, vacancy.Status);
    }

    [Fact]
    public void FullWorkflow_ArchivedToPublished_ShouldWork()
    {
        var vacancy = CreateArchivedVacancy();

        var publishResult = vacancy.Publish();

        Assert.True(publishResult.IsSuccess);
        Assert.Equal(VacancyStatus.Published, vacancy.Status);
        Assert.NotNull(vacancy.PublishedAt);
    }

    #endregion

    #region Helper Methods

    private Vacancy CreatePublishedVacancy()
    {
        var vacancy = Vacancy.CreateAndRegister(
            _validTitle,
            _validDescription,
            _validSalary,
            _validCompanyId,
            _validLocation,
            _validRecruiterInfo).Value;
        var categoryId = new CategoryId();
        vacancy.UpdateCategoryId(categoryId);
        vacancy.Publish();
        return vacancy;
    }

    private Vacancy CreateArchivedVacancy()
    {
        var vacancy = CreatePublishedVacancy();
        vacancy.Archive();
        return vacancy;
    }

    #endregion
}
