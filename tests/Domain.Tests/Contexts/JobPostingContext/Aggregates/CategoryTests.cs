using Domain.Abstraction.Interfaces;
using Domain.Contexts.JobPostingContext.Aggregates;
using Domain.Contexts.JobPostingContext.IDs;
using Moq;

namespace Domain.Tests.Contexts.JobPostingContext.Aggregates;

public class CategoryTests
{
    private readonly Mock<IUniqueCategoryNameSpecification> _uniqueNameSpecificationMock;

    public CategoryTests()
    {
        _uniqueNameSpecificationMock = new Mock<IUniqueCategoryNameSpecification>();
        SetupUniqueNameSpecificationMock();
    }

    private void SetupUniqueNameSpecificationMock()
    {
        _uniqueNameSpecificationMock.Setup(x => x.IsSatisfiedBy(It.IsAny<string>()))
                                   .Returns(true);
    }

    #region Create Tests

    [Theory]
    [MemberData(nameof(ValidCategoryNames))]
    public void Create_WithValidName_ShouldReturnSuccess(string validName)
    {
        var expectedName = validName.Trim();
        var expectedNormalizedName = expectedName.ToUpper().Replace(" ", string.Empty);

        var result = Category.Create(validName, _uniqueNameSpecificationMock.Object);

        Assert.True(result.IsSuccess);
        Assert.Equal(expectedName, result.Value.Name);
        Assert.Equal(expectedNormalizedName, result.Value.NormalizedName);
        Assert.NotNull(result.Value.Id);
        _uniqueNameSpecificationMock.Verify(x => x.IsSatisfiedBy(expectedNormalizedName), Times.Once);
    }

    [Theory]
    [MemberData(nameof(InvalidCategoryNames))]
    public void Create_WithInvalidName_ShouldReturnFailure(string invalidName)
    {
        var result = Category.Create(invalidName, _uniqueNameSpecificationMock.Object);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Create_WithDuplicateName_ShouldReturnFailure()
    {
        var duplicateName = "Software Development";
        var normalizedDuplicateName = duplicateName.ToUpper().Replace(" ", string.Empty);
        _uniqueNameSpecificationMock.Setup(x => x.IsSatisfiedBy(normalizedDuplicateName))
                                   .Returns(false);

        var result = Category.Create(duplicateName, _uniqueNameSpecificationMock.Object);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        _uniqueNameSpecificationMock.Verify(x => x.IsSatisfiedBy(normalizedDuplicateName), Times.Once);
    }

    [Fact]
    public void Create_WithWhitespaceAroundName_ShouldTrimAndReturnSuccess()
    {
        var nameWithWhitespace = "  Software Development  ";
        var expectedName = "Software Development";
        var expectedNormalizedName = "SOFTWAREDEVELOPMENT";

        var result = Category.Create(nameWithWhitespace, _uniqueNameSpecificationMock.Object);

        Assert.True(result.IsSuccess);
        Assert.Equal(expectedName, result.Value.Name);
        Assert.Equal(expectedNormalizedName, result.Value.NormalizedName);
        _uniqueNameSpecificationMock.Verify(x => x.IsSatisfiedBy(expectedNormalizedName), Times.Once);
    }

    [Fact]
    public void Create_WithMixedCaseName_ShouldNormalizeCorrectly()
    {
        var mixedCaseName = "SoftWare DeveLopment";
        var expectedNormalizedName = "SOFTWAREDEVELOPMENT";

        var result = Category.Create(mixedCaseName, _uniqueNameSpecificationMock.Object);

        Assert.True(result.IsSuccess);
        Assert.Equal(mixedCaseName, result.Value.Name);
        Assert.Equal(expectedNormalizedName, result.Value.NormalizedName);
        _uniqueNameSpecificationMock.Verify(x => x.IsSatisfiedBy(expectedNormalizedName), Times.Once);
    }

    #endregion

    #region UpdateName Tests

    [Fact]
    public void UpdateName_WithValidName_ShouldReturnSuccess()
    {
        var category = Category.Create("Original Name", _uniqueNameSpecificationMock.Object).Value;
        var newName = "Updated Name";
        var expectedNormalizedName = "UPDATEDNAME";

        var result = category.UpdateName(newName, _uniqueNameSpecificationMock.Object);

        Assert.True(result.IsSuccess);
        Assert.Equal(newName, category.Name);
        Assert.Equal(expectedNormalizedName, category.NormalizedName);
        _uniqueNameSpecificationMock.Verify(x => x.IsSatisfiedBy(expectedNormalizedName), Times.Once);
    }

    [Theory]
    [MemberData(nameof(InvalidCategoryNames))]
    public void UpdateName_WithInvalidName_ShouldReturnFailure(string invalidName)
    {
        var originalName = "Original Name";
        var category = Category.Create(originalName, _uniqueNameSpecificationMock.Object).Value;

        var result = category.UpdateName(invalidName, _uniqueNameSpecificationMock.Object);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(originalName, category.Name);
    }

    [Fact]
    public void UpdateName_WithDuplicateName_ShouldReturnFailure()
    {
        var originalName = "Original Name";
        var duplicateName = "Duplicate Name";
        var normalizedDuplicateName = duplicateName.ToUpper().Replace(" ", string.Empty);
        var category = Category.Create(originalName, _uniqueNameSpecificationMock.Object).Value;
        
        _uniqueNameSpecificationMock.Setup(x => x.IsSatisfiedBy(normalizedDuplicateName))
                                   .Returns(false);

        var result = category.UpdateName(duplicateName, _uniqueNameSpecificationMock.Object);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(originalName, category.Name);
        _uniqueNameSpecificationMock.Verify(x => x.IsSatisfiedBy(normalizedDuplicateName), Times.Once);
    }

    [Fact]
    public void UpdateName_WithWhitespaceAroundName_ShouldTrimAndReturnSuccess()
    {
        var category = Category.Create("Original Name", _uniqueNameSpecificationMock.Object).Value;
        var nameWithWhitespace = "  Updated Name  ";
        var expectedName = "Updated Name";
        var expectedNormalizedName = "UPDATEDNAME";

        var result = category.UpdateName(nameWithWhitespace, _uniqueNameSpecificationMock.Object);

        Assert.True(result.IsSuccess);
        Assert.Equal(expectedName, category.Name);
        Assert.Equal(expectedNormalizedName, category.NormalizedName);
    }

    #endregion

    #region Normalization Tests

    [Theory]
    [MemberData(nameof(NormalizationTestData))]
    public void Create_ShouldNormalizeNameCorrectly(string inputName, string expectedNormalizedName)
    {
        var result = Category.Create(inputName, _uniqueNameSpecificationMock.Object);

        Assert.True(result.IsSuccess);
        Assert.Equal(expectedNormalizedName, result.Value.NormalizedName);
        _uniqueNameSpecificationMock.Verify(x => x.IsSatisfiedBy(expectedNormalizedName), Times.Once);
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
