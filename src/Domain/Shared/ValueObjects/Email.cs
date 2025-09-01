using System.Text.RegularExpressions;
using Domain.Abstraction;
using Domain.Shared.ErrorHandling;

namespace Domain.Shared.ValueObjects;

public class Email : ValueObject
{
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
            return Result<Email>.Failure(new Error("Email.InvalidAddress", "Email address cannot be null or empty."));
        }

        if (!Regex.IsMatch(address, EmailPattern))
        {
            return Result<Email>.Failure(new Error("Email.InvalidFormat", "Email address format is invalid."));
        }

        var email = new Email(address);
        return Result<Email>.Success(email);
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Address;
    }
}