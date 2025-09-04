using Domain.Abstraction;
using Domain.Abstraction.Interfaces;
using Domain.Contexts.IdentityContext.Entities;
using Domain.Contexts.IdentityContext.Enums;
using Domain.Contexts.IdentityContext.IDs;
using Domain.Shared.ErrorHandling;
using Domain.Shared.ValueObjects;

namespace Domain.Contexts.IdentityContext.Aggregates;

public class User : AggregateRoot<UserId>
{
    public const int MaxFirstNameLength = 50;
    public const int MaxLastNameLength = 50;

    private User()
        : base(new UserId())
    {
    }

    private User(string firstName, string lastName, Email email, PhoneNumber phoneNumber, UserRole role)
        : base(new UserId())
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        Role = role;
    }

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public Email Email { get; private set; }

    public PhoneNumber PhoneNumber { get; private set; }

    public Account? Account { get; private set; }

    public UserRole Role { get; private set; }

    public Result CreateAccount(string password, IPasswordHasher passwordHasher)
    {
        if (Account != null)
        {
            return Result.Failure(Error.Conflict("User.AccountAlreadyExists", "User already has an associated account."));
        }

        var accountResult = Account.Create(Id, password, passwordHasher);
        if (accountResult.IsFailure)
        {
            return Result.Failure(accountResult.Error);
        }

        Account = accountResult.Value;
        return Result.Success();
    }

    public Result UpdateFirstName(string newFirstName)
    {
        if (string.IsNullOrWhiteSpace(newFirstName))
        {
            return Result.Failure(Error.Problem("User.InvalidFirstName", "First name cannot be null or empty."));
        }

        FirstName = newFirstName;
        return Result.Success();
    }

    public Result UpdateLastName(string newLastName)
    {
        if (string.IsNullOrWhiteSpace(newLastName))
        {
            return Result.Failure(Error.Problem("User.InvalidLastName", "Last name cannot be null or empty."));
        }

        LastName = newLastName;
        return Result.Success();
    }

    public Result UpdatePassword(string newPassword, IPasswordHasher passwordHasher)
    {
        if (Account == null)
        {
            return Result.Failure(Error.Conflict("User.NoAssociatedAccount", "User does not have an associated account."));
        }

        return Account.UpdatePassword(newPassword, passwordHasher);
    }

    public Result DeleteAccount()
    {
        if (Account == null)
        {
            return Result.Failure(Error.Conflict("User.NoAssociatedAccount", "User does not have an associated account."));
        }

        Account = null;
        return Result.Success();
    }

    internal static Result<User> Create(string firstName, string lastName, UserRole role, Email email, PhoneNumber phoneNumber)
    {
        var validationResult = ValidateCreationParameters(firstName, lastName, role, email, phoneNumber);
        if (validationResult.IsFailure)
        {
            return Result<User>.Failure(validationResult.Error);
        }

        var user = new User(firstName, lastName, email, phoneNumber, role);
        return Result<User>.Success(user);
    }

    internal Result UpdateEmail(Email newEmail)
    {
        if (newEmail is null)
        {
            return Result.Failure(Error.Problem("User.NullEmail", "Email cannot be null."));
        }

        Email = newEmail;
        return Result.Success();
    }

    internal Result UpdatePhoneNumber(PhoneNumber newPhoneNumber)
    {
        if (newPhoneNumber is null)
        {
            return Result.Failure(Error.Problem("User.NullPhoneNumber", "Phone number cannot be null."));
        }

        PhoneNumber = newPhoneNumber;
        return Result.Success();
    }

    private static Result ValidateCreationParameters(string firstName, string LastName, UserRole role, Email email, PhoneNumber phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            return Result.Failure(Error.Problem("User.InvalidFirstName", "First name cannot be null or empty."));
        }

        if (firstName.Length > MaxFirstNameLength)
        {
            return Result.Failure(Error.Problem("User.FirstNameTooLong", $"First name cannot exceed {MaxFirstNameLength} characters."));
        }

        if (string.IsNullOrWhiteSpace(LastName))
        {
            return Result.Failure(Error.Problem("User.InvalidLastName", "Last name cannot be null or empty."));
        }

        if (LastName.Length > MaxLastNameLength)
        {
            return Result.Failure(Error.Problem("User.LastNameTooLong", $"Last name cannot exceed {MaxLastNameLength} characters."));
        }

        if (email is null)
        {
            return Result.Failure(Error.Problem("User.NullEmail", "Email cannot be null."));
        }

        if (phoneNumber is null)
        {
            return Result.Failure(Error.Problem("User.NullPhoneNumber", "Phone number cannot be null."));
        }

        if (role == UserRole.Admin)
        {
            return Result.Failure(Error.Problem("User.InaccesibleRole", "Cannot create user with Admin role."));
        }

        return Result.Success();
    }
}
