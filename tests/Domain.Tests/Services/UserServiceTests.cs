using Domain.Abstraction.Interfaces;
using Domain.Contexts.IdentityContext.Aggregates;
using Domain.Contexts.IdentityContext.Enums;
using Domain.Repos.Users;
using Domain.Services;
using Domain.Shared.ValueObjects;
using Moq;

namespace Domain.Tests.Services;

public class UserServiceTests
{
    private Email _validEmail;
    private PhoneNumber _validPhoneNumber;
    private string _validPassword;
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _validEmail = Email.Create("test@example.com").Value;
        _validPhoneNumber = PhoneNumber.Create("+14156667777", "US").Value;
        _validPassword = "ValidPassword123!";
        _userService = new UserService(_userRepositoryMock.Object);

        _userRepositoryMock.Setup(x => x.IsUniqueEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _userRepositoryMock.Setup(x => x.IsUniquePhoneNumberAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

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

    #region Create Tests

    [Fact]
    public async Task CreateUserAsync_WithValidEmailPhoneNumberAndRole_ShouldReturnSuccess()
    {
        var result = await _userService.CreateUserAsync("John", "Doe", UserRole.JobSeeker, _validEmail, _validPhoneNumber, _validPassword, _passwordHasherMock.Object, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("John", result.Value.FirstName);
        Assert.Equal("Doe", result.Value.LastName);
        Assert.Equal(UserRole.JobSeeker, result.Value.Role);
        Assert.Equal(_validEmail, result.Value.Email);
        Assert.Equal(_validPhoneNumber, result.Value.PhoneNumber);
        Assert.True(_passwordHasherMock.Object.VerifyHashedPassword(result.Value.PasswordHash, _validPassword));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task CreateUserAsync_WithInvalidFirstName_ShouldReturnFailure(string firstName)
    {
        var result = await _userService.CreateUserAsync(firstName, "Doe", UserRole.JobSeeker, _validEmail, _validPhoneNumber, _validPassword, _passwordHasherMock.Object, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task CreateUserAsync_WithInvalidLastName_ShouldReturnFailure(string lastName)
    {
        var result = await _userService.CreateUserAsync("John", lastName, UserRole.JobSeeker, _validEmail, _validPhoneNumber, _validPassword, _passwordHasherMock.Object, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public async Task CreateUserAsync_WithFirstNameExceedingMaxLength_ShouldReturnFailure()
    {
        var longFirstName = new string('a', User.MaxFirstNameLength + 1);

        var result = await _userService.CreateUserAsync(longFirstName, "Doe", UserRole.JobSeeker, _validEmail, _validPhoneNumber, _validPassword, _passwordHasherMock.Object, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public async Task CreateUserAsync_WithLastNameExceedingMaxLength_ShouldReturnFailure()
    {
        var longLastName = new string('a', User.MaxLastNameLength + 1);

        var result = await _userService.CreateUserAsync("John", longLastName, UserRole.JobSeeker, _validEmail, _validPhoneNumber, _validPassword, _passwordHasherMock.Object, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public async Task CreateUser_WithDuplicateEmail_ShouldReturnFailure()
    {
        _userRepositoryMock.Setup(x => x.IsUniqueEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _userService.CreateUserAsync("John", "Doe", UserRole.JobSeeker, _validEmail, _validPhoneNumber, _validPassword, _passwordHasherMock.Object,  CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public async Task CreateUser_WithDuplicatePhoneNumber_ShouldReturnFailure()
    {
        _userRepositoryMock.Setup(x => x.IsUniquePhoneNumberAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _userService.CreateUserAsync("John", "Doe", UserRole.JobSeeker, _validEmail, _validPhoneNumber, _validPassword, _passwordHasherMock.Object, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public async Task CreateUserAsync_WithNullEmail_ShouldReturnFailure()
    {
        var result = await _userService.CreateUserAsync("John", "Doe", UserRole.JobSeeker, null!, _validPhoneNumber, _validPassword, _passwordHasherMock.Object, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public async Task CreateUserAsync_WithNullPhoneNumber_ShouldReturnFailure()
    {
        var result = await _userService.CreateUserAsync("John", "Doe", UserRole.JobSeeker, _validEmail, null!, _validPassword, _passwordHasherMock.Object, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public async Task CreateUserAsync_WithAdminRole_ShouldReturnFailure()
    {
        var result = await _userService.CreateUserAsync("Admin", "User", UserRole.Admin, _validEmail, _validPhoneNumber, _validPassword, _passwordHasherMock.Object, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task CreateUserAsync_WithInvalidPassword_ShouldReturnFailure(string password)
    {
        var result = await _userService.CreateUserAsync("John", "Doe", UserRole.JobSeeker, _validEmail, _validPhoneNumber, password, _passwordHasherMock.Object, CancellationToken.None);
        
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Theory]
    [MemberData(nameof(PasswordsNotInRange))]
    public async Task CreateUserAsync_WithPasswordNotInRange_ShouldReturnFailure(string password)
    {
        var result = await _userService.CreateUserAsync("John", "Doe", UserRole.JobSeeker, _validEmail, _validPhoneNumber, password, _passwordHasherMock.Object, CancellationToken.None);
        
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    #endregion

    #region UpdateEmail Tests

    [Fact]
    public async Task UpdateEmail_WithValidEmail_ShouldReturnSuccess()
    {
        var user = (await _userService.CreateUserAsync("John", "Doe", UserRole.JobSeeker, _validEmail, _validPhoneNumber, _validPassword, _passwordHasherMock.Object, CancellationToken.None)).Value;
        var newEmail = Email.Create("new@example.com").Value;

        var result = await _userService.UpdateUserEmailAsync(user, newEmail, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(newEmail, user.Email);
    }

    [Fact]
    public async Task UpdateEmail_WithNullEmail_ShouldReturnFailure()
    {
        var user = (await _userService.CreateUserAsync("John", "Doe", UserRole.JobSeeker, _validEmail, _validPhoneNumber, _validPassword, _passwordHasherMock.Object, CancellationToken.None)).Value;

        var result = await _userService.UpdateUserEmailAsync(user, null!, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(_validEmail, user.Email);
    }

    #endregion

    #region UpdatePhoneNumber Tests

    [Fact]
    public async Task UpdatePhoneNumber_WithValidPhoneNumber_ShouldReturnSuccess()
    {
        var user = (await _userService.CreateUserAsync("John", "Doe", UserRole.JobSeeker, _validEmail, _validPhoneNumber, _validPassword, _passwordHasherMock.Object, CancellationToken.None)).Value;
        var newPhone = PhoneNumber.Create("+380501234567", "UA").Value;

        var result = await _userService.UpdateUserPhoneNumberAsync(user, newPhone, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(newPhone, user.PhoneNumber);
    }

    [Fact]
    public async Task UpdatePhoneNumber_WithNullPhoneNumber_ShouldReturnFailure()
    {
        var user = (await _userService.CreateUserAsync("John", "Doe", UserRole.JobSeeker, _validEmail, _validPhoneNumber, _validPassword, _passwordHasherMock.Object, CancellationToken.None)).Value;

        var result = await _userService.UpdateUserPhoneNumberAsync(user, null!, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(_validPhoneNumber, user.PhoneNumber);
    }

    #endregion

    public static readonly TheoryData<string> PasswordsNotInRange = new()
    {
        "Short1!",
        new string('a', 101) + "1!"
    };
}
