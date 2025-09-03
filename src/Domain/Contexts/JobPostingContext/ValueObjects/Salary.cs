using System.Text.RegularExpressions;
using Domain.Abstraction;
using Domain.Shared.ErrorHandling;

namespace Domain.Contexts.JobPostingContext.ValueObjects;

public class Salary : ValueObject
{
    public const string CurrencyPattern = "^[A-Z]{3}$";

    private Salary(decimal minAmount, decimal maxAmount, string currency)
    {
        MinAmount = minAmount;
        MaxAmount = maxAmount;
        Currency = currency;
    }

    public decimal MinAmount { get; private init; }

    public decimal MaxAmount { get; private init; }

    public string Currency { get; private init; }

    public static Result<Salary> Range(decimal minAmount, decimal maxAmount, string currency)
    {
        if (minAmount < 0)
        {
            return Result<Salary>.Failure(new Error("Salary.InvalidMinAmount", "Minimum salary amount cannot be negative."));
        }

        if (maxAmount < minAmount)
        {
            return Result<Salary>.Failure(new Error("Salary.InvalidMaxAmount", "Maximum salary amount cannot be less than minimum amount."));
        }

        if (!IsValidCurrency(currency))
        {
            return Result<Salary>.Failure(new Error("Salary.InvalidCurrency", "Currency cannot be null or empty."));
        }

        var salary = new Salary(minAmount, maxAmount, currency.Trim().ToUpperInvariant());
        return Result<Salary>.Success(salary);
    }

    public static Result<Salary> Fixed(decimal amount, string currency)
    {
        if (amount < 0)
        {
            return Result<Salary>.Failure(new Error("Salary.InvalidAmount", "Salary amount cannot be negative."));
        }

        if (!IsValidCurrency(currency))
        {
            return Result<Salary>.Failure(new Error("Salary.InvalidCurrency", "Currency cannot be null or empty."));
        }

        var salary = new Salary(amount, amount, currency.Trim().ToUpperInvariant());
        return Result<Salary>.Success(salary);
    }

    public static Result<Salary> None()
    {
        var salary = new Salary(0, 0, "NNA");
        return Result<Salary>.Success(salary);
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return MinAmount;
        yield return MaxAmount;
        yield return Currency;
    }

    private static bool IsValidCurrency(string currency)
    {
        return !string.IsNullOrWhiteSpace(currency) && Regex.IsMatch(currency, CurrencyPattern);
    }
}
