using Domain.Abstraction.Interfaces;
using Domain.Contexts.IdentityContext.Aggregates;
using Domain.Contexts.IdentityContext.Enums;
using Domain.Repos.Users;
using Domain.Services;
using Domain.Shared.ValueObjects;
using Moq;

namespace Domain.Tests.Contexts.IdentityContext.Aggregates;

public class UserTests
{
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
    private Email _validEmail;
    private PhoneNumber _validPhoneNumber;
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly UserService _userService;
    private string _validPassword;

    public UserTests()
    {
        _userService = new UserService(_userRepositoryMock.Object);
        _validEmail = Email.Create("test@example.com").Value;
        _validPhoneNumber = PhoneNumber.Create("+14156667777", "US").Value;
        _validPassword = "ValidPass123!";
        SetupMocks();
    }

    private void SetupMocks()
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

        _userRepositoryMock.Setup(x => x.IsUniqueEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _userRepositoryMock.Setup(x => x.IsUniquePhoneNumberAsync(It.IsAny<PhoneNumber>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
    }

    #region UpdateFirstName Tests

    [Fact]
    public async Task UpdateFirstName_WithValidName_ShouldReturnSuccess()
    {
        var user = (await _userService.CreateUserAsync(
            "John",
            "Doe",
            UserRole.JobSeeker,
            _validEmail,
            _validPhoneNumber,
            _validPassword,
            _passwordHasherMock.Object,
            CancellationToken.None)).Value;
        var newName = "Jane";

        var result = user.UpdateFirstName(newName);

        Assert.True(result.IsSuccess);
        Assert.Equal(newName, user.FirstName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task UpdateFirstName_WithInvalidName_ShouldReturnFailure(string firstName)
    {
        var originalFirstName = "John";
        var user = (await _userService.CreateUserAsync(
            originalFirstName,
            "Doe",
            UserRole.JobSeeker,
            _validEmail,
            _validPhoneNumber,
            _validPassword,
            _passwordHasherMock.Object,
            CancellationToken.None)).Value;

        var result = user.UpdateFirstName(firstName);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(originalFirstName, user.FirstName);
    }

    #endregion

    #region UpdateLastName Tests

    [Fact]
    public async Task UpdateLastName_WithValidName_ShouldReturnSuccess()
    {
        var email = Email.Create("test@example.com").Value;
        var user = (await _userService.CreateUserAsync(
            "John",
            "Doe",
            UserRole.JobSeeker,
            _validEmail,
            _validPhoneNumber,
            _validPassword,
            _passwordHasherMock.Object,
            CancellationToken.None)).Value;
        var newLastName = "Smith";

        var result = user.UpdateLastName(newLastName);

        Assert.True(result.IsSuccess);
        Assert.Equal(newLastName, user.LastName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task UpdateLastName_WithInvalidName_ShouldReturnFailure(string lastName)
    {
        var originalLastName = "Doe";
        var user = (await _userService.CreateUserAsync(
            "John",
            originalLastName,
            UserRole.JobSeeker,
            _validEmail,
            _validPhoneNumber,
            _validPassword,
            _passwordHasherMock.Object,
            CancellationToken.None)).Value;

        var result = user.UpdateLastName(lastName);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(originalLastName, user.LastName);
    }

    #endregion

    #region UpdatePassword Tests

    [Fact]
    public async Task UpdatePassword_WithValidPassword_ShouldReturnSuccess()
    {
        var user = (await _userService.CreateUserAsync(
           "John",
           "Doe",
           UserRole.JobSeeker,
           _validEmail,
           _validPhoneNumber,
           _validPassword,
           _passwordHasherMock.Object,
           CancellationToken.None)).Value;
        string newPassword = "NewPassword456!";

        var result = user.UpdatePassword(newPassword, _passwordHasherMock.Object);

        Assert.True(result.IsSuccess);
        Assert.True(_passwordHasherMock.Object.VerifyHashedPassword(user.PasswordHash, newPassword));
        _passwordHasherMock.Verify(x => x.HashPassword(newPassword), Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task UpdatePassword_WithInvalidPassword_ShouldReturnFailure(string password)
    {
        var user = (await _userService.CreateUserAsync(
           "John",
           "Doe",
           UserRole.JobSeeker,
           _validEmail,
           _validPhoneNumber,
           _validPassword,
           _passwordHasherMock.Object,
           CancellationToken.None)).Value;
        string originalPassword = "OldPassword123!";

        var result = user.UpdatePassword(password, _passwordHasherMock.Object);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.True(_passwordHasherMock.Object.VerifyHashedPassword(user.PasswordHash, originalPassword));
    }

    [Theory]
    [MemberData(nameof(PasswordsNotInRange))]
    public async Task UpdatePassword_WithPasswordNotInRange_ShouldReturnFailure(string password)
    {
        var user = (await _userService.CreateUserAsync(
           "John",
           "Doe",
           UserRole.JobSeeker,
           _validEmail,
           _validPhoneNumber,
           _validPassword,
           _passwordHasherMock.Object,
           CancellationToken.None)).Value;
        string originalPassword = "OldPassword123!";

        var result = user.UpdatePassword(password, _passwordHasherMock.Object);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.True(_passwordHasherMock.Object.VerifyHashedPassword(user.PasswordHash, originalPassword));
    }

    #endregion

    public static readonly TheoryData<string> PasswordsNotInRange = new()
    {
        new string('a', User.MinPasswordLength - 1),
        new string('a', User.MaxPasswordLength + 1)
    };
}
