using Domain.Contexts.JobPostingContext.ValueObjects;
using Domain.Shared.ValueObjects;

namespace Domain.Tests.Contexts.JobPostingContext.ValueObjects;

public class RecruiterInfoTests
{
    [Fact]
    public void Create_WithValidEmailAndPhoneNumber_ShouldReturnSuccess()
    {
        var email = Email.Create("test@example.com").Value;
        var phoneNumber = PhoneNumber.Create("+14156667777", "US").Value;

        var result = RecruiterInfo.Create(email, phoneNumber);

        Assert.True(result.IsSuccess);
        Assert.Equal(email, result.Value.Email);
        Assert.Equal(phoneNumber, result.Value.PhoneNumber);
    }

    [Fact]
    public void Create_WithNullEmail_ShouldReturnFailure()
    {
        var phoneNumber = PhoneNumber.Create("+14156667777", "US").Value;

        var result = RecruiterInfo.Create(null!, phoneNumber);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Create_WithNullPhoneNumber_ShouldReturnFailure()
    {
        var email = Email.Create("test@example.com").Value;

        var result = RecruiterInfo.Create(email, null!);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Equals_WithSameValues_ShouldReturnTrue()
    {
        var email = Email.Create("test@example.com").Value;
        var phoneNumber = PhoneNumber.Create("+14156667777", "US").Value;

        var info1 = RecruiterInfo.Create(email, phoneNumber).Value;
        var info2 = RecruiterInfo.Create(email, phoneNumber).Value;

        Assert.True(info1.Equals(info2));
    }

    [Fact]
    public void Equals_WithDifferentValues_ShouldReturnTrue()
    {
        var email1 = Email.Create("test1@example.com").Value;
        var phoneNumber1 = PhoneNumber.Create("+14156667777", "US").Value;
        var email2 = Email.Create("test2@example.com").Value;
        var phoneNumber2 = PhoneNumber.Create("+441234567890", "GB").Value;

        var info1 = RecruiterInfo.Create(email1, phoneNumber1).Value;
        var info2 = RecruiterInfo.Create(email2, phoneNumber2).Value;

        Assert.False(info1.Equals(info2));
    }
}
