using System.Text.RegularExpressions;
using Domain.Abstraction;
using Domain.Shared.ErrorHandling;

namespace Domain.Shared.ValueObjects;

public class Email : ValueObject
{
    public const int MaxLength = 256;

    private const string EmailPattern = @"^((?!\.)[\w-_.]*[^.])(@\w+)(\.\w+(\.\w+)?[^.\W])$";

    private Email(string address)
    {
        Address = address;
    }

    public string Address { get; }

    public static Result<Email> Create(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            return Result<Email>.Failure(Error.Problem("Email.InvalidAddress", "Email address cannot be null or empty."));
        }

        if (address.Length > MaxLength)
        {
            return Result<Email>.Failure(Error.Problem("Email.AddressTooLong", $"Email address cannot exceed {MaxLength} characters."));
        }

        if (!Regex.IsMatch(address, EmailPattern))
        {
            return Result<Email>.Failure(Error.Problem("Email.InvalidFormat", "Email address format is invalid."));
        }

        var email = new Email(address);
        return Result<Email>.Success(email);
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Address;
    }
}