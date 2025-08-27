using System.Text.RegularExpressions;
using Domain.Abstraction;
using Domain.Shared.ErrorHandling;

namespace Domain.Shared.ValueObjects;

public class PhoneNumber : ValueObject
{
    private const string PhonePattern = @"^\+([0-9]{1,4})[-\s]?([0-9]{1,15})$";

    private PhoneNumber(string number)
    {
        Number = number;
    }

    public string Number { get; }

    public static Result<PhoneNumber> Create(string number)
    {
        if (string.IsNullOrWhiteSpace(number))
        {
            return Result<PhoneNumber>.Failure(new Error("PhoneNumber.InvalidNumber", "Phone number cannot be null or empty."));
        }

        if (!Regex.IsMatch(number, PhonePattern))
        {
            return Result<PhoneNumber>.Failure(new Error("PhoneNumber.InvalidFormat", "Phone number format is invalid."));
        }

        var phoneNumber = new PhoneNumber(number);
        return Result<PhoneNumber>.Success(phoneNumber);
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Number;
    }
}
