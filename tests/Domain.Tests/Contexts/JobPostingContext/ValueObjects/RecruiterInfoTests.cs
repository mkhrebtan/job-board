using Domain.Contexts.JobPostingContext.ValueObjects;
using Domain.Shared.ValueObjects;

namespace Domain.Tests.Contexts.JobPostingContext.ValueObjects;

public class RecruiterInfoTests
{
    [Fact]
    public void Create_WithValidInfo_ShouldReturnSuccess()
    {
        var firstName = "John";
        var email = Email.Create("test@example.com").Value;
        var phoneNumber = PhoneNumber.Create("+14156667777", "US").Value;

        var result = RecruiterInfo.Create(firstName, email, phoneNumber);

        Assert.True(result.IsSuccess);
        Assert.Equal(firstName, result.Value.FirstName);
        Assert.Equal(email, result.Value.Email);
        Assert.Equal(phoneNumber, result.Value.PhoneNumber);
    }

    [Fact]
    public void Create_WithNullFirstName_ShouldReturnFailure()
    {
        var email = Email.Create("test@example.com").Value;
        var phoneNumber = PhoneNumber.Create("+14156667777", "US").Value;

        var result = RecruiterInfo.Create(null!, email, phoneNumber);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Create_WithNullEmail_ShouldReturnFailure()
    {
        var firstName = "John";
        var phoneNumber = PhoneNumber.Create("+14156667777", "US").Value;

        var result = RecruiterInfo.Create(firstName, null!, phoneNumber);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Create_WithNullPhoneNumber_ShouldReturnFailure()
    {
        var firstName = "John";
        var email = Email.Create("test@example.com").Value;

        var result = RecruiterInfo.Create(firstName, email, null!);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Create_WithFirstNameExceedingMaxLength_ShouldReturnFailure()
    {
        var firstName = new string('A', RecruiterInfo.MaxFirstNameLength + 1);
        var email = Email.Create("test@example.com").Value;

        var result = RecruiterInfo.Create(firstName, email, null!);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Equals_WithSameValues_ShouldReturnTrue()
    {
        var firstName = "John";
        var email = Email.Create("test@example.com").Value;
        var phoneNumber = PhoneNumber.Create("+14156667777", "US").Value;

        var info1 = RecruiterInfo.Create(firstName, email, phoneNumber).Value;
        var info2 = RecruiterInfo.Create(firstName, email, phoneNumber).Value;

        Assert.True(info1.Equals(info2));
    }

    [Fact]
    public void Equals_WithDifferentValues_ShouldReturnTrue()
    {
        var firstName1 = "John";
        var email1 = Email.Create("test1@example.com").Value;
        var phoneNumber1 = PhoneNumber.Create("+14156667777", "US").Value;
        var firstName2 = "Jane";
        var email2 = Email.Create("test2@example.com").Value;
        var phoneNumber2 = PhoneNumber.Create("+441234567890", "GB").Value;

        var info1 = RecruiterInfo.Create(firstName1, email1, phoneNumber1).Value;
        var info2 = RecruiterInfo.Create(firstName2, email2, phoneNumber2).Value;

        Assert.False(info1.Equals(info2));
    }
}
