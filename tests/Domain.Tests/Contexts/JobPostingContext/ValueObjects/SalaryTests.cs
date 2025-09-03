using Domain.Contexts.JobPostingContext.ValueObjects;

namespace Domain.Tests.Contexts.JobPostingContext.ValueObjects;

public class SalaryTests
{
    [Fact]
    public void Range_WithValidInputs_ShouldReturnSuccess()
    {
        var minAmount = 50000m;
        var maxAmount = 80000m;
        var currency = "USD";

        var result = Salary.Range(minAmount, maxAmount, currency);

        Assert.True(result.IsSuccess);
        Assert.Equal(minAmount, result.Value.MinAmount);
        Assert.Equal(maxAmount, result.Value.MaxAmount);
        Assert.Equal(currency, result.Value.Currency);
    }

    [Fact]
    public void Range_WithNegativeMinAmount_ShouldReturnFailure()
    {
        var minAmount = -1000m;
        var maxAmount = 80000m;
        var currency = "USD";

        var result = Salary.Range(minAmount, maxAmount, currency);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Range_WithMaxAmountLessThanMinAmount_ShouldReturnFailure()
    {
        var minAmount = 80000m;
        var maxAmount = 50000m;
        var currency = "USD";

        var result = Salary.Range(minAmount, maxAmount, currency);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Range_WithInvalidCurrency_ShouldReturnFailure(string currency)
    {
        var minAmount = 50000m;
        var maxAmount = 80000m;

        var result = Salary.Range(minAmount, maxAmount, currency);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Fixed_WithValidInputs_ShouldReturnSuccess()
    {
        var amount = 75000m;
        var currency = "EUR";

        var result = Salary.Fixed(amount, currency);

        Assert.True(result.IsSuccess);
        Assert.Equal(amount, result.Value.MinAmount);
        Assert.Equal(amount, result.Value.MaxAmount);
        Assert.Equal(currency, result.Value.Currency);
    }

    [Fact]
    public void Fixed_WithNegativeAmount_ShouldReturnFailure()
    {
        var amount = -5000m;
        var currency = "USD";

        var result = Salary.Fixed(amount, currency);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Fixed_WithInvalidCurrency_ShouldReturnFailure(string currency)
    {
        var amount = 75000m;

        var result = Salary.Fixed(amount, currency);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void None_ShouldReturnSuccessWithZeroAmountsAndNotApplicableCurrency()
    {
        var result = Salary.None();

        Assert.True(result.IsSuccess);
        Assert.Equal(0, result.Value.MinAmount);
        Assert.Equal(0, result.Value.MaxAmount);
    }

    [Fact]
    public void Equals_WithSameValues_ShouldReturnTrue()
    {
        var salary1 = Salary.Range(50000m, 80000m, "USD").Value;
        var salary2 = Salary.Range(50000m, 80000m, "USD").Value;

        Assert.True(salary1.Equals(salary2));
    }

    [Fact]
    public void Equals_WithDifferentValues_ShouldReturnFalse()
    {
        var salary1 = Salary.Range(50000m, 80000m, "USD").Value;
        var salary2 = Salary.Range(60000m, 90000m, "USD").Value;

        Assert.False(salary1.Equals(salary2));
    }
}
