using Domain.Abstraction;
using Domain.Contexts.IdentityContext.Aggregates;
using Domain.Shared.ErrorHandling;
using Domain.Shared.ValueObjects;

namespace Domain.Contexts.JobPostingContext.ValueObjects;

public class RecruiterInfo : ValueObject
{
    public const int MaxFirstNameLength = User.MaxFirstNameLength;

    private RecruiterInfo(string firstName, Email email, PhoneNumber phoneNumber)
    {
        FirstName = firstName;
        Email = email;
        PhoneNumber = phoneNumber;
    }

    public string FirstName { get; private init; }

    public Email Email { get; private init; }

    public PhoneNumber PhoneNumber { get; private init; }

    public static Result<RecruiterInfo> Create(string firstName, Email email, PhoneNumber phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            return Result<RecruiterInfo>.Failure(new Error("RecruiterInfo.InvalidFirstName", "First name cannot be null or empty."));
        }

        if (firstName.Length > MaxFirstNameLength)
        {
            return Result<RecruiterInfo>.Failure(new Error("RecruiterInfo.FirstNameTooLong", $"First name cannot exceed {MaxFirstNameLength} characters."));
        }

        if (email is null)
        {
            return Result<RecruiterInfo>.Failure(new Error("RecruiterInfo.NullEmail", "Email cannot be null."));
        }

        if (phoneNumber is null)
        {
            return Result<RecruiterInfo>.Failure(new Error("RecruiterInfo.NullPhoneNumber", "Phone number cannot be null."));
        }

        var recruiterInfo = new RecruiterInfo(firstName, email, phoneNumber);
        return Result<RecruiterInfo>.Success(recruiterInfo);
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Email;
        yield return PhoneNumber;
    }
}
