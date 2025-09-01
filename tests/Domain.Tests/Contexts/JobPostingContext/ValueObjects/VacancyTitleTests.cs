using Domain.Contexts.JobPostingContext.ValueObjects;

namespace Domain.Tests.Contexts.JobPostingContext.ValueObjects;

public class VacancyTitleTests
{
    [Fact]
    public void Create_WithValidTitle_ShouldReturnSuccess()
    {
        var validTitle = "Software Engineer";

        var result = VacancyTitle.Create(validTitle);

        Assert.True(result.IsSuccess);
        Assert.Equal(validTitle, result.Value.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithInvalidTitle_ShouldReturnFailure(string title)
    {
        var result = VacancyTitle.Create(title);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Create_TitleExceedsMaxLength_ShouldReturnFailure()
    {
        var longTitle = new string('A', VacancyTitle.MaxLength + 1);

        var result = VacancyTitle.Create(longTitle);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Create_TitleAtMaxLength_ShouldReturnSuccess()
    {
        var titleAtMaxLength = new string('A', VacancyTitle.MaxLength);

        var result = VacancyTitle.Create(titleAtMaxLength);

        Assert.True(result.IsSuccess);
        Assert.Equal(titleAtMaxLength, result.Value.Value);
    }

    [Fact]
    public void Equals_SameTitle_ShouldReturnTrue()
    {
        var validTitle = "Software Engineer";

        var title1 = VacancyTitle.Create(validTitle).Value;
        var title2 = VacancyTitle.Create(validTitle).Value;

        Assert.True(title1.Equals(title2));
    }

    [Fact]
    public void Equals_DifferentTitle_ShouldReturnFalse()
    {
        var title1 = VacancyTitle.Create("Software Engineer").Value;
        var title2 = VacancyTitle.Create("Product Manager").Value;

        Assert.False(title1.Equals(title2));
    }
}