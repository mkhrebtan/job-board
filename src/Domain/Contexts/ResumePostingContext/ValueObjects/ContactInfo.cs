using Domain.Abstraction;
using Domain.Shared.ErrorHandling;
using Domain.Shared.ValueObjects;

namespace Domain.Contexts.ResumePostingContext.ValueObjects;

public class ContactInfo : ValueObject
{
    private ContactInfo()
    {
    }

    private ContactInfo(Email email, PhoneNumber phoneNumber)
    {
        Email = email;
        PhoneNumber = phoneNumber;
    }

    public Email Email { get; private init; }

    public PhoneNumber PhoneNumber { get; private init; }

    public static Result<ContactInfo> Create(Email email, PhoneNumber phoneNumber)
    {
        if (email is null)
        {
            return Result<ContactInfo>.Failure(new Error("ContactInfo.NullEmail", "Email cannot be null."));
        }

        if (phoneNumber is null)
        {
            return Result<ContactInfo>.Failure(new Error("ContactInfo.NullPhoneNumber", "Phone number cannot be null."));
        }

        var info = new ContactInfo(email, phoneNumber);
        return Result<ContactInfo>.Success(info);
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Email;
        yield return PhoneNumber;
    }
}