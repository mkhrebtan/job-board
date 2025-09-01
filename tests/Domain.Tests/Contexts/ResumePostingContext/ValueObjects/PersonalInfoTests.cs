using Domain.Contexts.ResumePostingContext.ValueObjects;

namespace Domain.Tests.Contexts.ResumePostingContext.ValueObjects;

public class PersonalInfoTests
{
    [Theory]
    [MemberData(nameof(ValidPersonalInfo))]
    public void Create_ValidPersonalInfo_ShouldReturnSuccess(string firstName, string lastName)
    {
        var result = PersonalInfo.Create(firstName, lastName);

        Assert.True(result.IsSuccess);
        Assert.Equal(firstName.Trim(), result.Value.FirstName);
        Assert.Equal(lastName.Trim(), result.Value.LastName);
    }

    [Theory]
    [MemberData(nameof(InvalidNames))]
    public void Create_InvalidFirstName_ShouldReturnFailure(string firstName)
    {
        var result = PersonalInfo.Create(firstName, "Doe");

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Theory]
    [MemberData(nameof(InvalidNames))]
    public void Create_InvalidLastName_ShouldReturnFailure(string lastName)
    {
        var result = PersonalInfo.Create("John", lastName);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Create_WithWhitespaceAroundNames_ShouldTrimAndReturnSuccess()
    {
        var firstNameWithWhitespace = "  John  ";
        var lastNameWithWhitespace = "  Doe  ";
        var expectedFirstName = "John";
        var expectedLastName = "Doe";

        var result = PersonalInfo.Create(firstNameWithWhitespace, lastNameWithWhitespace);

        Assert.True(result.IsSuccess);
        Assert.Equal(expectedFirstName, result.Value.FirstName);
        Assert.Equal(expectedLastName, result.Value.LastName);
    }

    [Theory]
    [MemberData(nameof(ValidPersonalInfo))]
    public void Equals_SamePersonalInfo_ShouldReturnTrue(string firstName, string lastName)
    {
        var info1 = PersonalInfo.Create(firstName, lastName).Value;
        var info2 = PersonalInfo.Create(firstName, lastName).Value;

        Assert.True(info1.Equals(info2));
    }

    [Fact]
    public void Equals_DifferentFirstName_ShouldReturnFalse()
    {
        var info1 = PersonalInfo.Create("John", "Doe").Value;
        var info2 = PersonalInfo.Create("Jane", "Doe").Value;

        Assert.False(info1.Equals(info2));
    }

    [Fact]
    public void Equals_DifferentLastName_ShouldReturnFalse()
    {
        var info1 = PersonalInfo.Create("John", "Doe").Value;
        var info2 = PersonalInfo.Create("John", "Smith").Value;

        Assert.False(info1.Equals(info2));
    }

    public static TheoryData<string, string> ValidPersonalInfo => new()
    {
        { "John", "Doe" },
        { "Jane", "Smith" },
        { "Michael", "Johnson" },
        { "Sarah", "Williams" },
        { "David", "Brown" },
        { "  John  ", "  Doe  " },
        { "A", "B" },
        { "Very Long First Name", "Very Long Last Name" }
    };

    public static TheoryData<string> InvalidNames => new()
    {
        "",
        "   ",
        null!
    };
}