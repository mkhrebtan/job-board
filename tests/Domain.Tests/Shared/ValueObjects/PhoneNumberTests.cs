using Domain.Shared.ValueObjects;

namespace Domain.Tests.Shared.ValueObjects;

public class PhoneNumberTests
{
    [Theory]
    [MemberData(nameof(ValidPhoneNumbers))]
    public void Create_ValidPhoneNumber_ShouldReturnSuccess(string validPhoneNumber, string regionCode)
    {
        var result = PhoneNumber.Create(validPhoneNumber, regionCode);

        Assert.True(result.IsSuccess);
        Assert.Equal(validPhoneNumber, result.Value.Number);
        Assert.Equal(regionCode, result.Value.RegionCode);
    }

    [Theory]
    [MemberData(nameof(InvalidPhoneNumbers))]
    public void Create_InvalidPhoneNumber_ShouldReturnFailure(string phoneNumber, string regionCode)
    {
        var result = PhoneNumber.Create(phoneNumber, regionCode);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Theory]
    [MemberData(nameof(InvalidRegionCodes))]
    public void Create_InvalidRegionCode_ShouldReturnFailure(string phoneNumber, string regionCode)
    {
        var result = PhoneNumber.Create(phoneNumber, regionCode);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Theory]
    [MemberData(nameof(ValidPhoneNumbers))]
    public void Equals_SamePhoneNumber_ShouldReturnTrue(string phoneNumber, string regionCode)
    {
        var phoneNumber1 = PhoneNumber.Create(phoneNumber, regionCode).Value;
        var phoneNumber2 = PhoneNumber.Create(phoneNumber, regionCode).Value;

        Assert.True(phoneNumber1.Equals(phoneNumber2));
    }

    [Fact]
    public void Equals_DifferentPhoneNumber_ShouldReturnFalse()
    {
        var phoneNumber1 = PhoneNumber.Create("+14156667777", "US").Value;
        var phoneNumber2 = PhoneNumber.Create("+441234567890", "GB").Value;

        Assert.False(phoneNumber1.Equals(phoneNumber2));
    }

    [Fact]
    public void Equals_SameNumberDifferentRegion_ShouldReturnFalse()
    {
        var phoneNumber1 = PhoneNumber.Create("+14156667777", "US").Value;
        var phoneNumber2 = PhoneNumber.Create("+14156667777", "CA").Value;

        Assert.False(phoneNumber1.Equals(phoneNumber2));
    }

    public static TheoryData<string, string> ValidPhoneNumbers => new()
    {
        { "+14156667777", "US" },
        { "+442034567890", "GB" },
        { "+380501234567", "UA" },
        { "+14185438090", "US" },
        { "+49 37322 123456", "DE" },
        { "+79876543210", "RU" }
    };

    public static TheoryData<string, string> InvalidPhoneNumbers => new()
    {
        { "1234567890", "US" },
        { "+", "US" },
        { "++1 1234567890", "US" },
        { "+1 abc123", "US" },
        { "+12345 123456789012345678", "US" },
        { "+12345-", "US" },
        { "+1 ", "US" },
        { "plain text", "US" },
        { string.Empty, "US" },
        { null!, "US" },
        { "   ", "US" },
        { "+1234567890123456789", "US" },
        { "+1 123-456-78901234567890", "US" }
    };

    public static TheoryData<string, string> InvalidRegionCodes => new()
    {
        { "+1234567890", "USA" },
        { "+1234567890", "u" },
        { "+1234567890", "us" },
        { "+1234567890", "U1" },
        { "+1234567890", "" },
        { "+1234567890", null! },
        { "+1234567890", "   " }
    };
}