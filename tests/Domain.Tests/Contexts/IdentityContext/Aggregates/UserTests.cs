using Domain.Abstraction.Interfaces;
using Domain.Contexts.IdentityContext.Aggregates;
using Domain.Contexts.IdentityContext.Entities;
using Domain.Contexts.IdentityContext.Enums;
using Domain.Shared.ValueObjects;
using Moq;

namespace Domain.Tests.Contexts.IdentityContext.Aggregates;

public class UserTests
{
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private Email _validEmail;
    private PhoneNumber _validPhoneNumber;

    public UserTests()
    {
        _passwordHasherMock = new Mock<IPasswordHasher>();
        SetupPasswordHasherMock();
        SetupData();
    }

    private void SetupPasswordHasherMock()
    {
        _passwordHasherMock.Setup(x => x.HashPassword(It.IsAny<string>()))
                          .Returns<string>(password =>
                          {
                              return password + "_hashed";
                          });

        _passwordHasherMock.Setup(x => x.VerifyHashedPassword(It.IsAny<string>(), It.IsAny<string>()))
                          .Returns<string, string>((hashedPassword, providedPassword) =>
                          {
                              return hashedPassword == providedPassword + "_hashed";
                          });
    }

    private void SetupData()
    {
        _validEmail = Email.Create("test@example.com").Value;
        _validPhoneNumber = PhoneNumber.Create("+14156667777", "US").Value;
    }

    #region Create Tests

    [Fact]
    public void Create_WithValidEmailPhoneNumberAndRole_ShouldReturnSuccess()
    {
        var result = User.Create("John", "Doe", UserRole.JobSeeker, _validEmail, _validPhoneNumber);

        Assert.True(result.IsSuccess);
        Assert.Equal("John", result.Value.FirstName);
        Assert.Equal("Doe", result.Value.LastName);
        Assert.Equal(UserRole.JobSeeker, result.Value.Role);
        Assert.Equal(_validEmail, result.Value.Email);
        Assert.Equal(_validPhoneNumber, result.Value.PhoneNumber);
        Assert.Null(result.Value.Account);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithInvalidFirstName_ShouldReturnFailure(string firstName)
    {
        var result = User.Create(firstName, "Doe", UserRole.JobSeeker, _validEmail, _validPhoneNumber);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithInvalidLastName_ShouldReturnFailure(string lastName)
    {
        var result = User.Create("John", lastName, UserRole.JobSeeker, _validEmail, _validPhoneNumber);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Create_WithFirstNameExceedingMaxLength_ShouldReturnFailure()
    {
        var longFirstName = new string('a', User.MaxFirstNameLength + 1);

        var result = User.Create(longFirstName, "Doe", UserRole.JobSeeker, _validEmail, _validPhoneNumber);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Create_WithLastNameExceedingMaxLength_ShouldReturnFailure()
    {
        var longLastName = new string('a', User.MaxLastNameLength + 1);

        var result = User.Create("John", longLastName, UserRole.JobSeeker, _validEmail, _validPhoneNumber);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Create_WithNullEmail_ShouldReturnFailure()
    {
        var result = User.Create("John", "Doe", UserRole.JobSeeker, null!, _validPhoneNumber);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Create_WithNullPhoneNumber_ShouldReturnFailure()
    {
        var result = User.Create("John", "Doe", UserRole.JobSeeker, _validEmail, null!);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Create_WithAdminRole_ShouldReturnFailure()
    {
        var result = User.Create("Admin", "User", UserRole.Admin, _validEmail, _validPhoneNumber);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    #endregion

    #region CreateAccount Tests

    [Fact]
    public void CreateAccount_WithValidPassword_ShouldReturnSuccess()
    {
        var user = User.Create("John", "Doe", UserRole.JobSeeker, _validEmail, _validPhoneNumber).Value;
        string password = "SecurePassword123!";

        var result = user.CreateAccount(password, _passwordHasherMock.Object);

        Assert.True(result.IsSuccess);
        Assert.NotNull(user.Account);
        Assert.Equal(user.Id, user.Account.UserId);
        Assert.True(_passwordHasherMock.Object.VerifyHashedPassword(user.Account.PasswordHash, password));
        _passwordHasherMock.Verify(x => x.HashPassword(password), Times.Once);
    }

    [Fact]
    public void CreateAccount_WhenAccountAlreadyExists_ShouldReturnFailure()
    {
        var user = User.Create("John", "Doe", UserRole.JobSeeker, _validEmail, _validPhoneNumber).Value;
        const string password = "SecurePassword123!";
        user.CreateAccount(password, _passwordHasherMock.Object);

        var result = user.CreateAccount("AnotherPassword", _passwordHasherMock.Object);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void CreateAccount_WithInvalidPassword_ShouldReturnFailure(string password)
    {
        var user = User.Create("John", "Doe", UserRole.JobSeeker, _validEmail, _validPhoneNumber).Value;

        var result = user.CreateAccount(password, _passwordHasherMock.Object);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Null(user.Account);
    }

    [Theory]
    [MemberData(nameof(PasswordsNotInRange))]
    public void CreateAccount_WithPasswordNotInRange_ShouldReturnFailure(string password)
    {
        var user = User.Create("John", "Doe", UserRole.JobSeeker, _validEmail, _validPhoneNumber).Value;

        var result = user.CreateAccount(password, _passwordHasherMock.Object);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Null(user.Account);
    }

    #endregion

    #region UpdateEmail Tests

    [Fact]
    public void UpdateEmail_WithValidEmail_ShouldReturnSuccess()
    {
        var user = User.Create("John", "Doe", UserRole.JobSeeker, _validEmail, _validPhoneNumber).Value;
        var newEmail = Email.Create("new@example.com").Value;

        var result = user.UpdateEmail(newEmail);

        Assert.True(result.IsSuccess);
        Assert.Equal(newEmail, user.Email);
    }

    [Fact]
    public void UpdateEmail_WithNullEmail_ShouldReturnFailure()
    {
        var user = User.Create("John", "Doe", UserRole.JobSeeker, _validEmail, _validPhoneNumber).Value;

        var result = user.UpdateEmail(null!);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(_validEmail, user.Email);
    }

    #endregion

    #region UpdatePhoneNumber Tests

    [Fact]
    public void UpdatePhoneNumber_WithValidPhoneNumber_ShouldReturnSuccess()
    {
        var user = User.Create("John", "Doe", UserRole.JobSeeker, _validEmail, _validPhoneNumber).Value;
        var newPhone = PhoneNumber.Create("+380501234567", "UA").Value;

        var result = user.UpdatePhoneNumber(newPhone);

        Assert.True(result.IsSuccess);
        Assert.Equal(newPhone, user.PhoneNumber);
    }

    [Fact]
    public void UpdatePhoneNumber_WithNullPhoneNumber_ShouldReturnFailure()
    {
        var user = User.Create("John", "Doe", UserRole.JobSeeker, _validEmail, _validPhoneNumber).Value;

        var result = user.UpdatePhoneNumber(null!);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(_validPhoneNumber, user.PhoneNumber);
    }

    #endregion

    #region UpdateFirstName Tests

    [Fact]
    public void UpdateFirstName_WithValidName_ShouldReturnSuccess()
    {
        var user = User.Create("John", "Doe", UserRole.JobSeeker, _validEmail, _validPhoneNumber).Value;
        var newName = "Jane";

        var result = user.UpdateFirstName(newName);

        Assert.True(result.IsSuccess);
        Assert.Equal(newName, user.FirstName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void UpdateFirstName_WithInvalidName_ShouldReturnFailure(string firstName)
    {
        var originalFirstName = "John";
        var user = User.Create(originalFirstName, "Doe", UserRole.JobSeeker, _validEmail, _validPhoneNumber).Value;

        var result = user.UpdateFirstName(firstName);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(originalFirstName, user.FirstName);
    }

    #endregion

    #region UpdateLastName Tests

    [Fact]
    public void UpdateLastName_WithValidName_ShouldReturnSuccess()
    {
        var email = Email.Create("test@example.com").Value;
        var user = User.Create("John", "Doe", UserRole.JobSeeker, _validEmail, _validPhoneNumber).Value;
        var newLastName = "Smith";

        var result = user.UpdateLastName(newLastName);

        Assert.True(result.IsSuccess);
        Assert.Equal(newLastName, user.LastName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void UpdateLastName_WithInvalidName_ShouldReturnFailure(string lastName)
    {
        var originalLastName = "Doe";
        var user = User.Create("John", originalLastName, UserRole.JobSeeker, _validEmail, _validPhoneNumber).Value;

        var result = user.UpdateLastName(lastName);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(originalLastName, user.LastName);
    }

    #endregion

    #region UpdatePassword Tests

    [Fact]
    public void UpdatePassword_WithValidPasswordAndExistingAccount_ShouldReturnSuccess()
    {
        var user = User.Create("John", "Doe", UserRole.JobSeeker, _validEmail, _validPhoneNumber).Value;
        user.CreateAccount("OldPassword123!", _passwordHasherMock.Object);
        string newPassword = "NewPassword456!";

        var result = user.UpdatePassword(newPassword, _passwordHasherMock.Object);

        Assert.True(result.IsSuccess);
        Assert.True(_passwordHasherMock.Object.VerifyHashedPassword(user.Account!.PasswordHash, newPassword));
        _passwordHasherMock.Verify(x => x.HashPassword(newPassword), Times.Once);
    }

    [Fact]
    public void UpdatePassword_WithoutAccount_ShouldReturnFailure()
    {
        var user = User.Create("John", "Doe", UserRole.JobSeeker, _validEmail, _validPhoneNumber).Value;

        var result = user.UpdatePassword("NewPassword123!", _passwordHasherMock.Object);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void UpdatePassword_WithInvalidPassword_ShouldReturnFailure(string password)
    {
        var user = User.Create("John", "Doe", UserRole.JobSeeker, _validEmail, _validPhoneNumber).Value;
        string originalPassword = "OldPassword123!";
        user.CreateAccount(originalPassword, _passwordHasherMock.Object);

        var result = user.UpdatePassword(password, _passwordHasherMock.Object);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.True(_passwordHasherMock.Object.VerifyHashedPassword(user.Account!.PasswordHash, originalPassword));
    }

    [Theory]
    [MemberData(nameof(PasswordsNotInRange))]
    public void UpdatePassword_WithPasswordNotInRange_ShouldReturnFailure(string password)
    {
        var user = User.Create("John", "Doe", UserRole.JobSeeker, _validEmail, _validPhoneNumber).Value;
        string originalPassword = "OldPassword123!";
        user.CreateAccount(originalPassword, _passwordHasherMock.Object);

        var result = user.UpdatePassword(password, _passwordHasherMock.Object);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.True(_passwordHasherMock.Object.VerifyHashedPassword(user.Account!.PasswordHash, originalPassword));
    }

    #endregion

    #region DeleteAccount Tests

    [Fact]
    public void DeleteAccount_WithExistingAccount_ShouldReturnSuccess()
    {
        var email = Email.Create("test@example.com").Value;
        var user = User.Create("John", "Doe", UserRole.JobSeeker, _validEmail, _validPhoneNumber).Value;
        user.CreateAccount("Password123!", _passwordHasherMock.Object);

        var result = user.DeleteAccount();

        Assert.True(result.IsSuccess);
        Assert.Null(user.Account);
    }

    [Fact]
    public void DeleteAccount_WithoutAccount_ShouldReturnFailure()
    {
        var email = Email.Create("test@example.com").Value;
        var user = User.Create("John", "Doe", UserRole.JobSeeker, _validEmail, _validPhoneNumber).Value;

        var result = user.DeleteAccount();

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    #endregion

    public static readonly TheoryData<string> PasswordsNotInRange = new()
    {
        new string('a', Account.MinPasswordLength - 1),
        new string('a', Account.MaxPasswordLength + 1)
    };
}
