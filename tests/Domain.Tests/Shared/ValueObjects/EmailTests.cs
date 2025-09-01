using Domain.Shared.ValueObjects;

namespace Domain.Tests.Shared.ValueObjects;

public class EmailTests
{
    [Theory]
    [MemberData(nameof(ValidEmails))]
    public void Create_ValidEmail_ShouldReturnSuccess(string validEmail)
    {
        var result = Email.Create(validEmail);

        Assert.True(result.IsSuccess);
        Assert.Equal(validEmail, result.Value.Address);
    }

    [Theory]
    [MemberData(nameof(InvalidEmails))]
    public void Create_InvalidEmail_ShouldReturnFailure(string email)
    {
        var result = Email.Create(email);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Theory]
    [MemberData(nameof(ValidEmails))]
    public void Equals_SameEmail_ShouldReturnTrue(string email)
    {
        var email1 = Email.Create(email).Value;
        var email2 = Email.Create(email).Value;

        Assert.True(email1.Equals(email2));
    }

    [Fact]
    public void Equals_DifferentEmail_ShouldReturnFalse()
    {
        var email1 = Email.Create("test1@example.com").Value;
        var email2 = Email.Create("test2@example.com").Value;

        Assert.False(email1.Equals(email2));
    }

    public static TheoryData<string> ValidEmails => new()
    {
        "tboulding6@dion.ne.jp",
        "mgager2@webnode.com",
        "amulmuray5@timesonline.co.uk",
    };

    public static TheoryData<string> InvalidEmails => new()
    {
        "plainaddress",
        "@missingusername.com",
        "username@.com",
        "username@.com.",
        "username@domain..com",
        "username@domain,com",
        "username@domain com",
        "username@-domain.com",
        "username@domain-.com",
        "username@domain.c",
        string.Empty,
        null!
    };
}
