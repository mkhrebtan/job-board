using Domain.Abstraction;
using Domain.Shared.ErrorHandling;

namespace Domain.Contexts.JobPostingContext.ValueObjects;

public class VacancyTitle : ValueObject
{
    public const int MaxLength = 100;

    private VacancyTitle(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<VacancyTitle> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<VacancyTitle>.Failure(new Error("Vacancy.InvalidTitle", "Title cannot be null or empty."));
        }

        if (value.Length > MaxLength)
        {
            return Result<VacancyTitle>.Failure(new Error("Vacancy.TitleTooLong", $"Title cannot exceed {MaxLength} characters."));
        }

        return Result<VacancyTitle>.Success(new VacancyTitle(value));
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}
