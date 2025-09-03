using System.Text.RegularExpressions;
using Domain.Abstraction;
using Domain.Shared.ErrorHandling;

namespace Domain.Contexts.ResumePostingContext.ValueObjects;

public class Money : ValueObject
{
    public const string CurrencyPattern = "^[A-Z]{3}$";

    private Money(decimal value, string currency)
    {
        Value = value;
        Currency = currency;
    }

    public decimal Value { get; }

    public string Currency { get; }

    public static Result<Money> Create(decimal value, string currency)
    {
        if (!IsValidCurrency(currency))
        {
            return Result<Money>.Failure(new Error("Money.EmptyCurrency", "Currency cannot be null or empty."));
        }

        if (value < 0)
        {
            return Result<Money>.Failure(new Error("Money.NegativeValue", "Money amount cannot be negative."));
        }

        return Result<Money>.Success(new Money(value, currency));
    }

    public static Result<Money> Zero(string currency)
    {
        return Create(0, currency);
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
        yield return Currency;
    }

    private static bool IsValidCurrency(string currency)
    {
        return !string.IsNullOrWhiteSpace(currency) && Regex.IsMatch(currency, CurrencyPattern);
    }
}
