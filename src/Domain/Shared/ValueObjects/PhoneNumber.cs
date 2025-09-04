using System.Text.RegularExpressions;
using Domain.Abstraction;
using Domain.Shared.ErrorHandling;
using PhoneNumbers;

namespace Domain.Shared.ValueObjects;

public class PhoneNumber : ValueObject
{
    public const int MaxNumberLength = 15;
    public const int RegionCodeLength = 2;

    private PhoneNumber(string number, string regionCode)
    {
        Number = number;
        RegionCode = regionCode;
    }

    public string Number { get; }

    public string RegionCode { get; }

    public static Result<PhoneNumber> Create(string number, string regionCode)
    {
        if (string.IsNullOrWhiteSpace(number))
        {
            return Result<PhoneNumber>.Failure(Error.Problem("PhoneNumber.InvalidNumber", "Phone number cannot be null or empty."));
        }

        if (number.Length > MaxNumberLength)
        {
            return Result<PhoneNumber>.Failure(Error.Problem("PhoneNumber.NumberTooLong", $"Phone number cannot exceed {MaxNumberLength} characters."));
        }

        if (string.IsNullOrWhiteSpace(regionCode) || !Regex.IsMatch(regionCode, @"^[A-Z]{2}$"))
        {
            return Result<PhoneNumber>.Failure(Error.Problem("PhoneNumber.InvalidRegionCode", "Region code must be a valid ISO 3166-1 alpha-2 code."));
        }

        var phoneUtil = PhoneNumberUtil.GetInstance();
        try
        {
            var parsedNumber = phoneUtil.Parse(number, regionCode);
            if (!phoneUtil.IsValidNumber(parsedNumber))
            {
                return Result<PhoneNumber>.Failure(Error.Problem("PhoneNumber.InvalidFormat", "Phone number format is invalid."));
            }
        }
        catch (NumberParseException)
        {
            return Result<PhoneNumber>.Failure(Error.Problem("PhoneNumber.InvalidFormat", "Phone number format is invalid."));
        }

        var phoneNumber = new PhoneNumber(number, regionCode);
        return Result<PhoneNumber>.Success(phoneNumber);
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Number;
        yield return RegionCode;
    }
}
