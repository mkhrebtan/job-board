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
    private User(string firstName, string lastName, Email? email, PhoneNumber? phoneNumber, UserRole role)
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

    public Email? Email { get; private set; }

    public PhoneNumber? PhoneNumber { get; private set; }

    public Account? Account { get; private set; }

    public UserRole Role { get; private set; }

    public static Result<User> Create(string firstName, string lastName, UserRole role, Email email)
    {
        return Create(firstName, lastName, role, email, null);
    }

    public static Result<User> Create(string firstName, string lastName, UserRole role, PhoneNumber phoneNumber)
    {
        return Create(firstName, lastName, role, null, phoneNumber);
    }

    public Result CreateAccount(string password, IPasswordHasher passwordHasher)
    {
        if (Account != null)
        {
            return Result.Failure(new Error("User.AccountAlreadyExists", "User already has an associated account."));
        }

        var accountResult = Account.Create(Id, password, passwordHasher);
        if (accountResult.IsFailure)
        {
            return Result.Failure(accountResult.Error);
        }

        Account = accountResult.Value;
        return Result.Success();
    }

    public Result UpdateEmail(Email newEmail)
    {
        if (newEmail is null)
        {
            return Result.Failure(new Error("User.NullEmail", "Email cannot be null."));
        }

        Email = newEmail;
        return Result.Success();
    }

    public Result UpdatePhoneNumber(PhoneNumber newPhoneNumber)
    {
        if (newPhoneNumber is null)
        {
            return Result.Failure(new Error("User.NullPhoneNumber", "Phone number cannot be null."));
        }

        PhoneNumber = newPhoneNumber;
        return Result.Success();
    }

    public Result UpdateFirstName(string newFirstName)
    {
        if (string.IsNullOrWhiteSpace(newFirstName))
        {
            return Result.Failure(new Error("User.InvalidFirstName", "First name cannot be null or empty."));
        }

        FirstName = newFirstName;
        return Result.Success();
    }

    public Result UpdateLastName(string newLastName)
    {
        if (string.IsNullOrWhiteSpace(newLastName))
        {
            return Result.Failure(new Error("User.InvalidLastName", "Last name cannot be null or empty."));
        }

        LastName = newLastName;
        return Result.Success();
    }

    public Result UpdatePassword(string newPassword, IPasswordHasher passwordHasher)
    {
        if (Account == null)
        {
            return Result.Failure(new Error("User.NoAssociatedAccount", "User does not have an associated account."));
        }

        return Account.UpdatePassword(newPassword, passwordHasher);
    }

    public Result DeleteAccount()
    {
        if (Account == null)
        {
            return Result.Failure(new Error("User.NoAssociatedAccount", "User does not have an associated account."));
        }

        Account = null;
        return Result.Success();
    }

    private static Result<User> Create(string firstName, string LastName, UserRole role, Email? email, PhoneNumber? phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            return Result<User>.Failure(new Error("User.InvalidFirstName", "First name cannot be null or empty."));
        }

        if (string.IsNullOrWhiteSpace(LastName))
        {
            return Result<User>.Failure(new Error("User.InvalidLastName", "Last name cannot be null or empty."));
        }

        if (email == null && phoneNumber is null)
        {
            return Result<User>.Failure(new Error("User.InvalidContactInfo", "At least one contact information (email or phone number) must be provided."));
        }

        if (role == UserRole.Admin)
        {
            return Result<User>.Failure(new Error("User.InaccesibleRole", "Cannot create user with Admin role."));
        }

        var user = new User(firstName, LastName, email, phoneNumber, role);
        return Result<User>.Success(user);
    }
}
