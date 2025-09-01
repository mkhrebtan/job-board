using Domain.Abstraction;
using Domain.Shared.ErrorHandling;

namespace Domain.Contexts.ResumePostingContext.ValueObjects;

public class DateRange : ValueObject
{
    private DateRange(DateTime startDate, DateTime? endDate)
    {
        StartDate = startDate;
        EndDate = endDate;
    }

    public DateTime StartDate { get; private set; }

    public DateTime? EndDate { get; private set; }

    public TimeSpan Duration => (EndDate ?? DateTime.UtcNow) - StartDate;

    public static Result<DateRange> Create(DateTime startDate, DateTime endDate)
    {
        if (endDate < startDate)
        {
            return Result<DateRange>.Failure(new Error("DateRange.InvalidEndDate", "End date cannot be earlier than start date."));
        }

        if (startDate.Year == endDate.Year && startDate.Month == endDate.Month)
        {
            return Result<DateRange>.Failure(new Error("DateRange.SameStartAndEndDate", "Start date and end date cannot be in the same month and year."));
        }

        return Result<DateRange>.Success(new DateRange(startDate, endDate));
    }

    public static Result<DateRange> Create(DateTime startDate)
    {
        return Result<DateRange>.Success(new DateRange(startDate, default));
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return StartDate;
        yield return EndDate ?? DateTime.MaxValue;
    }
}
