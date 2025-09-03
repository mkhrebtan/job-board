using Domain.Contexts.ResumePostingContext.ValueObjects;

namespace Domain.Tests.Contexts.ResumePostingContext.ValueObjects;

public class DesiredPositionTests
{
    [Fact]
    public void Create_ValidPosition_ShouldReturnSuccess()
    {
        var validPosition = "Software Engineer";

        var result = DesiredPosition.Create(validPosition);

        Assert.True(result.IsSuccess);
        Assert.Equal(validPosition.Trim(), result.Value.Title);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_InvalidPosition_ShouldReturnFailure(string position)
    {
        var result = DesiredPosition.Create(position);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Create_WithWhitespaceAroundTitle_ShouldTrimAndReturnSuccess()
    {
        var titleWithWhitespace = "  Software Engineer  ";
        var expectedTitle = "Software Engineer";

        var result = DesiredPosition.Create(titleWithWhitespace);

        Assert.True(result.IsSuccess);
        Assert.Equal(expectedTitle, result.Value.Title);
    }

    [Fact]
    public void Create_WhenExceedingMaxLength_ShouldReturnFailure()
    {
        var longPosition = new string('a', DesiredPosition.MaxLength + 1);

        var result = DesiredPosition.Create(longPosition);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Equals_SamePosition_ShouldReturnTrue()
    {
        var position = "Software Engineer";

        var position1 = DesiredPosition.Create(position).Value;
        var position2 = DesiredPosition.Create(position).Value;

        Assert.True(position1.Equals(position2));
    }

    [Fact]
    public void Equals_DifferentPosition_ShouldReturnFalse()
    {
        var position1 = DesiredPosition.Create("Software Engineer").Value;
        var position2 = DesiredPosition.Create("Product Manager").Value;

        Assert.False(position1.Equals(position2));
    }
}