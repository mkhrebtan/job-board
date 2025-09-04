using Domain.Abstraction;
using Domain.Abstraction.Interfaces;
using Domain.Contexts.IdentityContext.IDs;
using Domain.Shared.ErrorHandling;

namespace Domain.Contexts.IdentityContext.Entities;

public class Account : Entity<AccountId>
{
    public const int MinPasswordLength = 8;
    public const int MaxPasswordLength = 100;

    private Account(UserId userId, string passwordHash)
        : base(new AccountId())
    {
        UserId = userId;
        PasswordHash = passwordHash;
    }

    public UserId UserId { get; private set; }

    public string PasswordHash { get; private set; }

    internal static Result<Account> Create(UserId userId, string password, IPasswordHasher passwordHasher)
    {
        if (userId == null || userId.Value == Guid.Empty)
        {
            return Result<Account>.Failure(Error.Problem("Account.InvalidUserId", "UserId cannot be null or empty."));
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            return Result<Account>.Failure(Error.Problem("Account.InvalidPassword", "Password cannot be null or empty."));
        }

        if (password.Length < 8 || password.Length > 100)
        {
            return Result<Account>.Failure(Error.Problem("Account.InvalidPasswordLength", $"Password must be between {MinPasswordLength} and {MaxPasswordLength} characters long."));
        }

        string passwordHash = passwordHasher.HashPassword(password);
        var account = new Account(userId, passwordHash);
        return Result<Account>.Success(account);
    }

    internal Result UpdatePassword(string newPassword, IPasswordHasher passwordHasher)
    {
        if (string.IsNullOrWhiteSpace(newPassword))
        {
            return Result.Failure(Error.Problem("Account.InvalidPassword", "New password cannot be null or empty."));
        }

        if (newPassword.Length < 8 || newPassword.Length > 100)
        {
            return Result.Failure(Error.Problem("Account.InvalidPasswordLength", $"Password must be between {MinPasswordLength} and {MaxPasswordLength} characters long."));
        }

        PasswordHash = passwordHasher.HashPassword(newPassword);
        return Result.Success();
    }
}
