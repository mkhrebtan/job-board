using Domain.Abstraction;
using Domain.Contexts.IdentityContext.Aggregates;
using Domain.Shared.ErrorHandling;

namespace Domain.Contexts.ResumePostingContext.ValueObjects;

public class PersonalInfo : ValueObject
{
    public const int MaxFirstNameLength = User.MaxFirstNameLength;
    public const int MaxLastNameLength = User.MaxLastNameLength;

    private PersonalInfo(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }

    public string FirstName { get; private init; }

    public string LastName { get; private init; }

    public static Result<PersonalInfo> Create(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            return Result<PersonalInfo>.Failure(Error.Problem("PersonalInfo.InvalidFirstName", "First name cannot be null or empty."));
        }

        if (firstName.Length > MaxFirstNameLength)
        {
            return Result<PersonalInfo>.Failure(Error.Problem("PersonalInfo.FirstNameTooLong", $"First name cannot exceed {MaxFirstNameLength} characters."));
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            return Result<PersonalInfo>.Failure(Error.Problem("PersonalInfo.InvalidLastName", "Last name cannot be null or empty."));
        }

        if (lastName.Length > MaxLastNameLength)
        {
            return Result<PersonalInfo>.Failure(Error.Problem("PersonalInfo.LastNameTooLong", $"Last name cannot exceed {MaxLastNameLength} characters."));
        }

        var info = new PersonalInfo(firstName.Trim(), lastName.Trim());
        return Result<PersonalInfo>.Success(info);
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return FirstName;
        yield return LastName;
    }
}