using Domain.Abstraction;
using Domain.Shared.ErrorHandling;

namespace Domain.Contexts.ResumePostingContext.ValueObjects;

public class PersonalInfo : ValueObject
{
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
            return Result<PersonalInfo>.Failure(new Error("PersonalInfo.InvalidFirstName", "First name cannot be null or empty."));
        }

        if (string.IsNullOrWhiteSpace(lastName))
        {
            return Result<PersonalInfo>.Failure(new Error("PersonalInfo.InvalidLastName", "Last name cannot be null or empty."));
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