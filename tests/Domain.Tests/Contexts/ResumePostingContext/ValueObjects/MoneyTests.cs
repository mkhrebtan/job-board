using Domain.Contexts.ResumePostingContext.ValueObjects;

namespace Domain.Tests.Contexts.ResumePostingContext.ValueObjects;

public class MoneyTests
{
    [Theory]
    [MemberData(nameof(ValidMoneyValues))]
    public void Create_ValidMoney_ShouldReturnSuccess(decimal value, string currency)
    {
        var result = Money.Create(value, currency);

        Assert.True(result.IsSuccess);
        Assert.Equal(value, result.Value.Value);
        Assert.Equal(currency, result.Value.Currency);
    }

    [Theory]
    [MemberData(nameof(InvalidCurrencies))]
    public void Create_InvalidCurrency_ShouldReturnFailure(string currency)
    {
        var result = Money.Create(100m, currency);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Theory]
    [MemberData(nameof(NegativeValues))]
    public void Create_NegativeValue_ShouldReturnFailure(decimal value)
    {
        var result = Money.Create(value, "USD");

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Create_ZeroValue_ShouldReturnSuccess()
    {
        var result = Money.Create(0m, "USD");

        Assert.True(result.IsSuccess);
        Assert.Equal(0m, result.Value.Value);
        Assert.Equal("USD", result.Value.Currency);
    }

    [Theory]
    [MemberData(nameof(ValidCurrencies))]
    public void Zero_WithValidCurrency_ShouldReturnSuccess(string currency)
    {
        var result = Money.Zero(currency);

        Assert.True(result.IsSuccess);
        Assert.Equal(0m, result.Value.Value);
        Assert.Equal(currency, result.Value.Currency);
    }

    [Theory]
    [MemberData(nameof(InvalidCurrencies))]
    public void Zero_WithInvalidCurrency_ShouldReturnFailure(string currency)
    {
        var result = Money.Zero(currency);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Equals_SameMoney_ShouldReturnTrue()
    {
        var money1 = Money.Create(100.50m, "USD").Value;
        var money2 = Money.Create(100.50m, "USD").Value;

        Assert.True(money1.Equals(money2));
    }

    [Fact]
    public void Equals_DifferentValue_ShouldReturnFalse()
    {
        var money1 = Money.Create(100m, "USD").Value;
        var money2 = Money.Create(200m, "USD").Value;

        Assert.False(money1.Equals(money2));
    }

    [Fact]
    public void Equals_DifferentCurrency_ShouldReturnFalse()
    {
        var money1 = Money.Create(100m, "USD").Value;
        var money2 = Money.Create(100m, "EUR").Value;

        Assert.False(money1.Equals(money2));
    }

    public static TheoryData<decimal, string> ValidMoneyValues => new()
    {
        { 0m, "USD" },
        { 100.50m, "USD" },
        { 1000m, "EUR" },
        { 0.01m, "GBP" },
        { 999999.99m, "JPY" },
        { 50000m, "UAH" }
    };

    public static TheoryData<string> ValidCurrencies => new()
    {
        "USD",
        "EUR",
        "GBP",
        "JPY",
        "UAH",
        "CAD",
        "AUD"
    };

    public static TheoryData<string> InvalidCurrencies => new()
    {
        "",
        "   ",
        null!
    };

    public static TheoryData<decimal> NegativeValues => new()
    {
        -0.01m,
        -100m,
        -1000.50m
    };
}