using Domain.Contexts.JobPostingContext.Aggregates;
using Domain.Repos.Categories;
using Domain.Services;
using Moq;

namespace Domain.Tests.Services;

public class CategoryServiceTests
{
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly CategoryService _categoryService;

    public CategoryServiceTests()
    {
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _categoryService = new CategoryService(_categoryRepositoryMock.Object);
        SetupUniqueNameSpecificationMock();
    }

    private void SetupUniqueNameSpecificationMock()
    {
        _categoryRepositoryMock.Setup(x => x.IsUniqueNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                                   .ReturnsAsync(true);
    }

    #region Create Tests

    [Theory]
    [MemberData(nameof(ValidCategoryNames))]
    public async Task CreateCategoryAsync_WithValidName_ShouldReturnSuccess(string validName)
    {
        var expectedName = validName.Trim();
        var expectedNormalizedName = expectedName.ToUpper().Replace(" ", string.Empty);

        var result = await _categoryService.CreateCategoryAsync(validName, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(expectedName, result.Value.Name);
        Assert.Equal(expectedNormalizedName, result.Value.NormalizedName);
        Assert.NotNull(result.Value.Id);
        _categoryRepositoryMock.Verify(x => x.IsUniqueNameAsync(expectedNormalizedName, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [MemberData(nameof(InvalidCategoryNames))]
    public async Task CreateCategoryAsync_WithInvalidName_ShouldReturnFailure(string invalidName)
    {
        var result = await _categoryService.CreateCategoryAsync(invalidName, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public async Task CreateCategoryAsync_WithDuplicateName_ShouldReturnFailure()
    {
        var duplicateName = "Software Development";
        var normalizedDuplicateName = duplicateName.ToUpper().Replace(" ", string.Empty);
        _categoryRepositoryMock.Setup(x => x.IsUniqueNameAsync(normalizedDuplicateName, It.IsAny<CancellationToken>()))
                                   .ReturnsAsync(false);

        var result = await _categoryService.CreateCategoryAsync(duplicateName, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        _categoryRepositoryMock.Verify(x => x.IsUniqueNameAsync(normalizedDuplicateName, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateCategoryAsync_WithWhitespaceAroundName_ShouldTrimAndReturnSuccess()
    {
        var nameWithWhitespace = "  Software Development  ";
        var expectedName = "Software Development";
        var expectedNormalizedName = "SOFTWAREDEVELOPMENT";

        var result = await _categoryService.CreateCategoryAsync(nameWithWhitespace, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(expectedName, result.Value.Name);
        Assert.Equal(expectedNormalizedName, result.Value.NormalizedName);
        _categoryRepositoryMock.Verify(x => x.IsUniqueNameAsync(expectedNormalizedName, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateCategoryAsync_WithMixedCaseName_ShouldNormalizeCorrectly()
    {
        var mixedCaseName = "SoftWare DeveLopment";
        var expectedNormalizedName = "SOFTWAREDEVELOPMENT";

        var result = await _categoryService.CreateCategoryAsync(mixedCaseName, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(mixedCaseName, result.Value.Name);
        Assert.Equal(expectedNormalizedName, result.Value.NormalizedName);
        _categoryRepositoryMock.Verify(x => x.IsUniqueNameAsync(expectedNormalizedName, It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region UpdateName Tests

    [Fact]
    public async Task UpdateCategoryNameAsync_WithValidName_ShouldReturnSuccess()
    {
        var category = (await _categoryService.CreateCategoryAsync("Original Name", CancellationToken.None)).Value;
        var newName = "Updated Name";
        var expectedNormalizedName = "UPDATEDNAME";

        var result = await _categoryService.UpdateCategoryNameAsync(category, newName, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(newName, category.Name);
        Assert.Equal(expectedNormalizedName, category.NormalizedName);
        _categoryRepositoryMock.Verify(x => x.IsUniqueNameAsync(expectedNormalizedName, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [MemberData(nameof(InvalidCategoryNames))]
    public async Task UpdateCategoryNameAsync_WithInvalidName_ShouldReturnFailure(string invalidName)
    {
        var originalName = "Original Name";
        var category = (await _categoryService.CreateCategoryAsync(originalName, CancellationToken.None)).Value;

        var result = await _categoryService.UpdateCategoryNameAsync(category, invalidName, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(originalName, category.Name);
    }

    [Fact]
    public async Task UpdateCategoryNameAsync_WithDuplicateName_ShouldReturnFailure()
    {
        var originalName = "Original Name";
        var duplicateName = "Duplicate Name";
        var normalizedDuplicateName = duplicateName.ToUpper().Replace(" ", string.Empty);
        var category = (await _categoryService.CreateCategoryAsync(originalName, CancellationToken.None)).Value;

        _categoryRepositoryMock.Setup(x => x.IsUniqueNameAsync(normalizedDuplicateName, It.IsAny<CancellationToken>()))
                                   .ReturnsAsync(false);

        var result = await _categoryService.UpdateCategoryNameAsync(category, duplicateName, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(originalName, category.Name);
        _categoryRepositoryMock.Verify(x => x.IsUniqueNameAsync(normalizedDuplicateName, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateCategoryNameAsync_WithWhitespaceAroundName_ShouldTrimAndReturnSuccess()
    {
        var category = (await _categoryService.CreateCategoryAsync("Original Name", CancellationToken.None)).Value;
        var nameWithWhitespace = "  Updated Name  ";
        var expectedName = "Updated Name";
        var expectedNormalizedName = "UPDATEDNAME";

        var result = await _categoryService.UpdateCategoryNameAsync(category, nameWithWhitespace, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(expectedName, category.Name);
        Assert.Equal(expectedNormalizedName, category.NormalizedName);
    }

    #endregion

    #region Normalization Tests

    [Theory]
    [MemberData(nameof(NormalizationTestData))]
    public async Task CreateCategoryAsync_ShouldNormalizeNameCorrectly(string inputName, string expectedNormalizedName)
    {
        var result = (await _categoryService.CreateCategoryAsync(inputName, CancellationToken.None));

        Assert.True(result.IsSuccess);
        Assert.Equal(expectedNormalizedName, result.Value.NormalizedName);
        _categoryRepositoryMock.Verify(x => x.IsUniqueNameAsync(expectedNormalizedName, It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region Test Data

    public static TheoryData<string> ValidCategoryNames => new()
    {
        "Software Development",
        "Marketing",
        "Human Resources",
        "Finance & Accounting",
        "Sales",
        "Customer Support",
        "Data Science",
        "UX/UI Design",
        "DevOps & Infrastructure",
        "Business Analysis",
        "Quality Assurance",
        "Product Management"
    };

    public static TheoryData<string> InvalidCategoryNames => new()
    {
        "",
        "   ",
        null!
    };

    public static TheoryData<string, string> NormalizationTestData => new()
    {
        { "Software Development", "SOFTWAREDEVELOPMENT" },
        { "software development", "SOFTWAREDEVELOPMENT" },
        { "SoftWare DeveLopment", "SOFTWAREDEVELOPMENT" },
        { "MARKETING", "MARKETING" },
        { "marketing", "MARKETING" },
        { "Human Resources", "HUMANRESOURCES" },
        { "Finance & Accounting", "FINANCE&ACCOUNTING" },
        { "ux/ui design", "UX/UIDESIGN" },
        { "  Trimmed Name  ", "TRIMMEDNAME" }
    };

    #endregion
}
