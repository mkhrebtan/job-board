using Domain.Abstraction.Interfaces;
using Domain.Contexts.IdentityContext.Aggregates;
using Domain.Contexts.IdentityContext.Enums;
using Domain.Repos.Users;
using Domain.Shared.ErrorHandling;
using Domain.Shared.ValueObjects;

namespace Domain.Services;

public class UserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<User>> CreateUserAsync(string firstName, string lastName, UserRole role, Email email, PhoneNumber phoneNumber, string password, IPasswordHasher passwordHasher, CancellationToken ct)
    {
        var emailValidation = await ValidateEmailAsync(email, ct);
        if (emailValidation.IsFailure)
        {
            return Result<User>.Failure(emailValidation.Error);
        }

        var phoneValidation = await ValidatePhoneNumberAsync(phoneNumber, ct);
        if (phoneValidation.IsFailure)
        {
            return Result<User>.Failure(phoneValidation.Error);
        }

        return User.Create(firstName, lastName, role, email, phoneNumber, password, passwordHasher);
    }

    public async Task<Result> UpdateUserEmailAsync(User user, Email newEmail, CancellationToken ct)
    {
        var emailValidation = await ValidateEmailAsync(newEmail, ct);
        if (emailValidation.IsFailure)
        {
            return emailValidation;
        }

        user.UpdateEmail(newEmail);
        return Result.Success();
    }

    public async Task<Result> UpdateUserPhoneNumberAsync(User user, PhoneNumber newPhoneNumber, CancellationToken ct)
    {
        var phoneValidation = await ValidatePhoneNumberAsync(newPhoneNumber, ct);
        if (phoneValidation.IsFailure)
        {
            return phoneValidation;
        }

        user.UpdatePhoneNumber(newPhoneNumber);
        return Result.Success();
    }

    private async Task<Result> ValidateEmailAsync(Email email, CancellationToken ct)
    {
        if (email == null)
        {
            return Result.Failure(Error.Problem("User.InvalidEmail", "Email cannot be null."));
        }

        if (!await _repository.IsUniqueEmailAsync(email.Address, ct))
        {
            return Result.Failure(Error.Conflict("User.DuplicateEmail", $"Email '{email.Address}' is already in use."));
        }

        return Result.Success();
    }

    private async Task<Result> ValidatePhoneNumberAsync(PhoneNumber phoneNumber, CancellationToken ct)
    {
        if (phoneNumber == null)
        {
            return Result.Failure(Error.Problem("User.InvalidPhoneNumber", "Phone number cannot be null."));
        }

        if (!await _repository.IsUniquePhoneNumberAsync(phoneNumber.Number, ct))
        {
            return Result.Failure(Error.Conflict("User.DuplicatePhoneNumber", $"Phone number '{phoneNumber.Number}' is already in use."));
        }

        return Result.Success();
    }
}
