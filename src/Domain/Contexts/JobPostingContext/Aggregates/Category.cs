using Domain.Abstraction;
using Domain.Abstraction.Interfaces;
using Domain.Contexts.JobPostingContext.IDs;
using Domain.Shared.ErrorHandling;

namespace Domain.Contexts.JobPostingContext.Aggregates;

public class Category : AggregateRoot<CategoryId>
{
    private Category(string name, string normalizedName)
        : base(new CategoryId())
    {
        Name = name;
        NormalizedName = normalizedName;
    }

    public string Name { get; private set; }

    public string NormalizedName { get; private set; }

    public static Result<Category> Create(string name, IUniqueCategoryNameSpecification specification)
    {
        var validationResult = ValidateName(name, specification, out string trimmedName, out string normalizedName);
        if (validationResult.IsFailure)
        {
            return Result<Category>.Failure(validationResult.Error);
        }

        var category = new Category(trimmedName, normalizedName);
        return Result<Category>.Success(category);
    }

    public Result UpdateName(string newName, IUniqueCategoryNameSpecification specification)
    {
        var validationResult = ValidateName(newName, specification, out string trimmedName, out string normalizedName);
        if (validationResult.IsFailure)
        {
            return Result.Failure(validationResult.Error);
        }

        Name = trimmedName;
        NormalizedName = normalizedName;
        return Result.Success();
    }

    private static Result ValidateName(
        string name,
        IUniqueCategoryNameSpecification specification,
        out string trimmedName,
        out string normalizedName)
    {
        trimmedName = string.Empty;
        normalizedName = string.Empty;
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure(new Error("Category.InvalidName", "Category name cannot be null or empty."));
        }

        trimmedName = name.Trim();
        normalizedName = trimmedName.ToUpper().Replace(" ", string.Empty);
        if (!specification.IsSatisfiedBy(normalizedName))
        {
            return Result.Failure(new Error("Category.DuplicateName", $"Category name '{name}' already exists."));
        }

        return Result.Success();
    }
}
