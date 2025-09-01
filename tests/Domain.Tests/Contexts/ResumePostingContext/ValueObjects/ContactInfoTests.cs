using Domain.Contexts.ResumePostingContext.ValueObjects;
using Domain.Shared.ValueObjects;

namespace Domain.Tests.Contexts.ResumePostingContext.ValueObjects;

public class ContactInfoTests
{
    [Fact]
    public void Create_WithValidEmailAndPhoneNumber_ShouldReturnSuccess()
    {
        var email = Email.Create("test@example.com").Value;
        var phoneNumber = PhoneNumber.Create("+14156667777", "US").Value;

        var result = ContactInfo.Create(email, phoneNumber);

        Assert.True(result.IsSuccess);
        Assert.Equal(email, result.Value.Email);
        Assert.Equal(phoneNumber, result.Value.PhoneNumber);
    }

    [Fact]
    public void Create_WithNullEmail_ShouldReturnFailure()
    {
        var phoneNumber = PhoneNumber.Create("+14156667777", "US").Value;

        var result = ContactInfo.Create(null, phoneNumber);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Create_WithNullPhoneNumber_ShouldReturnFailure()
    {
        var email = Email.Create("test@example.com").Value;

        var result = ContactInfo.Create(email, null);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Create_WithBothNull_ShouldReturnFailure()
    {
        var result = ContactInfo.Create(null, null);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Equals_SameContactInfo_ShouldReturnTrue()
    {
        var email = Email.Create("test@example.com").Value;
        var phoneNumber = PhoneNumber.Create("+14156667777", "US").Value;

        var contactInfo1 = ContactInfo.Create(email, phoneNumber).Value;
        var contactInfo2 = ContactInfo.Create(email, phoneNumber).Value;

        Assert.True(contactInfo1.Equals(contactInfo2));
    }

    [Fact]
    public void Equals_DifferentEmail_ShouldReturnFalse()
    {
        var email1 = Email.Create("test1@example.com").Value;
        var email2 = Email.Create("test2@example.com").Value;
        var phoneNumber = PhoneNumber.Create("+14156667777", "US").Value;

        var contactInfo1 = ContactInfo.Create(email1, phoneNumber).Value;
        var contactInfo2 = ContactInfo.Create(email2, phoneNumber).Value;

        Assert.False(contactInfo1.Equals(contactInfo2));
    }

    [Fact]
    public void Equals_DifferentPhoneNumber_ShouldReturnFalse()
    {
        var email = Email.Create("test@example.com").Value;
        var phoneNumber1 = PhoneNumber.Create("+14156667777", "US").Value;
        var phoneNumber2 = PhoneNumber.Create("+442034567890", "GB").Value;
        var contactInfo1 = ContactInfo.Create(email, phoneNumber1).Value;
        var contactInfo2 = ContactInfo.Create(email, phoneNumber2).Value;

        Assert.False(contactInfo1.Equals(contactInfo2));
    }
}