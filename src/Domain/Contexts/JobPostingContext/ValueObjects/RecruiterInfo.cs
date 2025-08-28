using Domain.Abstraction;
using Domain.Shared.ErrorHandling;
using Domain.Shared.ValueObjects;

namespace Domain.Contexts.JobPostingContext.ValueObjects;

public class RecruiterInfo : ValueObject
{
    private RecruiterInfo(Email email, PhoneNumber phoneNumber)
    {
        Email = email;
        PhoneNumber = phoneNumber;
    }

    public Email Email { get; private init; }

    public PhoneNumber PhoneNumber { get; private init; }

    public static Result<RecruiterInfo> Create(Email email, PhoneNumber phoneNumber)
    {
        if (email is null)
        {
            return Result<RecruiterInfo>.Failure(new Error("RecruiterInfo.NullEmail", "Email cannot be null."));
        }

        if (phoneNumber is null)
        {
            return Result<RecruiterInfo>.Failure(new Error("RecruiterInfo.NullPhoneNumber", "Phone number cannot be null."));
        }

        var recruiterInfo = new RecruiterInfo(email, phoneNumber);
        return Result<RecruiterInfo>.Success(recruiterInfo);
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Email;
        yield return PhoneNumber;
    }
}
