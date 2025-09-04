using Domain.Abstraction.Interfaces;
using Domain.Contexts.IdentityContext.Aggregates;
using Domain.Contexts.IdentityContext.Entities;
using Domain.Contexts.IdentityContext.Enums;
using Domain.Repos.Users;
using Domain.Services;
using Domain.Shared.ValueObjects;
using Moq;
using System.Threading.Tasks;

namespace Domain.Tests.Contexts.IdentityContext.Aggregates;

public class UserTests
{
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
    private Email _validEmail;
    private PhoneNumber _validPhoneNumber;
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly UserService _userService;

    public UserTests()
    {
        _userService = new UserService(_userRepositoryMock.Object);
        _validEmail = Email.Create("test@example.com").Value;
        _validPhoneNumber = PhoneNumber.Create("+14156667777", "US").Value;
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

        _userRepositoryMock.Setup(x => x.IsUniqueEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _userRepositoryMock.Setup(x => x.IsUniquePhoneNumberAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
    }

    #region CreateAccount Tests

    [Fact]
    public async Task CreateAccount_WithValidPassword_ShouldReturnSuccess()
    {
        var user = (await _userService.CreateUserAsync("John", "Doe", UserRole.JobSeeker, _validEmail, _validPhoneNumber, CancellationToken.None)).Value;
        string password = "SecurePassword123!";

        var result = user.CreateAccount(password, _passwordHasherMock.Object);

        Assert.True(result.IsSuccess);
        Assert.NotNull(user.Account);
        Assert.Equal(user.Id, user.Account.UserId);
        Assert.True(_passwordHasherMock.Object.VerifyHashedPassword(user.Account.PasswordHash, password));
        _passwordHasherMock.Verify(x => x.HashPassword(password), Times.Once);
    }

    [Fact]
    public async Task CreateAccount_WhenAccountAlreadyExists_ShouldReturnFailure()
    {
        var user = (await _userService.CreateUserAsync("John", "Doe", UserRole.JobSeeker, _validEmail, _validPhoneNumber, CancellationToken.None)).Value;
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
    public async Task CreateAccount_WithInvalidPassword_ShouldReturnFailure(string password)
    {
        var user = (await _userService.CreateUserAsync("John", "Doe", UserRole.JobSeeker, _validEmail, _validPhoneNumber, CancellationToken.None)).Value;

        var result = user.CreateAccount(password, _passwordHasherMock.Object);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Null(user.Account);
    }

    [Theory]
    [MemberData(nameof(PasswordsNotInRange))]
    public async Task CreateAccount_WithPasswordNotInRange_ShouldReturnFailure(string password)
    {
        var user = (await _userService.CreateUserAsync("John", "Doe", UserRole.JobSeeker, _validEmail, _validPhoneNumber, CancellationToken.None)).Value;

        var result = user.CreateAccount(password, _passwordHasherMock.Object);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Null(user.Account);
    }

    #endregion

    #region UpdateFirstName Tests

    [Fact]
    public async Task UpdateFirstName_WithValidName_ShouldReturnSuccess()
    {
        var user = (await _userService.CreateUserAsync("John", "Doe", UserRole.JobSeeker, _validEmail, _validPhoneNumber, CancellationToken.None)).Value;
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
        var user = (await _userService.CreateUserAsync(originalFirstName, "Doe", UserRole.JobSeeker, _validEmail, _validPhoneNumber, CancellationToken.None)).Value;

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
        var user = (await _userService.CreateUserAsync("John", "Doe", UserRole.JobSeeker, _validEmail, _validPhoneNumber, CancellationToken.None)).Value;
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
        var user = (await _userService.CreateUserAsync("John", originalLastName, UserRole.JobSeeker, _validEmail, _validPhoneNumber, CancellationToken.None)).Value;

        var result = user.UpdateLastName(lastName);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(originalLastName, user.LastName);
    }

    #endregion

    #region UpdatePassword Tests

    [Fact]
    public async Task UpdatePassword_WithValidPasswordAndExistingAccount_ShouldReturnSuccess()
    {
        var user = (await _userService.CreateUserAsync("John", "Doe", UserRole.JobSeeker, _validEmail, _validPhoneNumber, CancellationToken.None)).Value;
        user.CreateAccount("OldPassword123!", _passwordHasherMock.Object);
        string newPassword = "NewPassword456!";

        var result = user.UpdatePassword(newPassword, _passwordHasherMock.Object);

        Assert.True(result.IsSuccess);
        Assert.True(_passwordHasherMock.Object.VerifyHashedPassword(user.Account!.PasswordHash, newPassword));
        _passwordHasherMock.Verify(x => x.HashPassword(newPassword), Times.Once);
    }

    [Fact]
    public async Task UpdatePassword_WithoutAccount_ShouldReturnFailure()
    {
        var user = (await _userService.CreateUserAsync("John", "Doe", UserRole.JobSeeker, _validEmail, _validPhoneNumber, CancellationToken.None)).Value;

        var result = user.UpdatePassword("NewPassword123!", _passwordHasherMock.Object);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task UpdatePassword_WithInvalidPassword_ShouldReturnFailure(string password)
    {
        var user = (await _userService.CreateUserAsync("John", "Doe", UserRole.JobSeeker, _validEmail, _validPhoneNumber, CancellationToken.None)).Value;
        string originalPassword = "OldPassword123!";
        user.CreateAccount(originalPassword, _passwordHasherMock.Object);

        var result = user.UpdatePassword(password, _passwordHasherMock.Object);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.True(_passwordHasherMock.Object.VerifyHashedPassword(user.Account!.PasswordHash, originalPassword));
    }

    [Theory]
    [MemberData(nameof(PasswordsNotInRange))]
    public async Task UpdatePassword_WithPasswordNotInRange_ShouldReturnFailure(string password)
    {
        var user = (await _userService.CreateUserAsync("John", "Doe", UserRole.JobSeeker, _validEmail, _validPhoneNumber, CancellationToken.None)).Value;
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
    public async Task DeleteAccount_WithExistingAccount_ShouldReturnSuccess()
    {
        var email = Email.Create("test@example.com").Value;
        var user = (await _userService.CreateUserAsync("John", "Doe", UserRole.JobSeeker, _validEmail, _validPhoneNumber, CancellationToken.None)).Value;
        user.CreateAccount("Password123!", _passwordHasherMock.Object);

        var result = user.DeleteAccount();

        Assert.True(result.IsSuccess);
        Assert.Null(user.Account);
    }

    [Fact]
    public async Task DeleteAccount_WithoutAccount_ShouldReturnFailure()
    {
        var email = Email.Create("test@example.com").Value;
        var user = (await _userService.CreateUserAsync("John", "Doe", UserRole.JobSeeker, _validEmail, _validPhoneNumber, CancellationToken.None)).Value;

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
