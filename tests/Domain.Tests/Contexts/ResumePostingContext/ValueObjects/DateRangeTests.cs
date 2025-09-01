using Domain.Contexts.ResumePostingContext.ValueObjects;

namespace Domain.Tests.Contexts.ResumePostingContext.ValueObjects;

public class DateRangeTests
{
    [Fact]
    public void Create_WithValidStartAndEndDate_ShouldReturnSuccess()
    {
        var startDate = new DateTime(2023, 1, 1);
        var endDate = new DateTime(2023, 12, 1);

        var result = DateRange.Create(startDate, endDate);

        Assert.True(result.IsSuccess);
        Assert.Equal(startDate, result.Value.StartDate);
        Assert.Equal(endDate, result.Value.EndDate);
    }

    [Fact]
    public void Create_WithOnlyStartDate_ShouldReturnSuccess()
    {
        var startDate = new DateTime(2023, 1, 1);

        var result = DateRange.Create(startDate);

        Assert.True(result.IsSuccess);
        Assert.Equal(startDate, result.Value.StartDate);
        Assert.Null(result.Value.EndDate);
    }

    [Fact]
    public void Create_WithEndDateBeforeStartDate_ShouldReturnFailure()
    {
        var startDate = new DateTime(2023, 12, 1);
        var endDate = new DateTime(2023, 1, 1);

        var result = DateRange.Create(startDate, endDate);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Create_WithSameStartAndEndDateYearAndMonth_ShouldReturnFailure()
    {
        var date = new DateTime(2023, 6, 1);

        var result = DateRange.Create(date, date);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
    }

    [Fact]
    public void Duration_WithEndDate_ShouldReturnCorrectTimeSpan()
    {
        var startDate = new DateTime(2023, 1, 1);
        var endDate = new DateTime(2023, 2, 1);
        var dateRange = DateRange.Create(startDate, endDate).Value;
        var expectedDuration = endDate - startDate;

        var duration = dateRange.Duration;

        Assert.Equal(expectedDuration, duration);
    }

    [Fact]
    public void Duration_WithoutEndDate_ShouldReturnTimeSpanToNow()
    {
        var startDate = DateTime.UtcNow.AddDays(-10);
        var dateRange = DateRange.Create(startDate).Value;

        var duration = dateRange.Duration;

        Assert.True(duration.TotalDays >= 9 && duration.TotalDays <= 11);
    }

    [Fact]
    public void Equals_SameDateRange_ShouldReturnTrue()
    {
        var startDate = new DateTime(2023, 1, 1);
        var endDate = new DateTime(2023, 12, 31);
        var dateRange1 = DateRange.Create(startDate, endDate).Value;
        var dateRange2 = DateRange.Create(startDate, endDate).Value;

        Assert.True(dateRange1.Equals(dateRange2));
    }

    [Fact]
    public void Equals_DifferentStartDate_ShouldReturnFalse()
    {
        var startDate1 = new DateTime(2023, 1, 1);
        var startDate2 = new DateTime(2023, 2, 1);
        var endDate = new DateTime(2023, 12, 31);
        var dateRange1 = DateRange.Create(startDate1, endDate).Value;
        var dateRange2 = DateRange.Create(startDate2, endDate).Value;

        Assert.False(dateRange1.Equals(dateRange2));
    }

    [Fact]
    public void Equals_SameOngoing_ShouldReturnTrue()
    {
        var startDate = new DateTime(2023, 1, 1);
        var dateRange1 = DateRange.Create(startDate).Value;
        var dateRange2 = DateRange.Create(startDate).Value;

        Assert.True(dateRange1.Equals(dateRange2));
    }
}