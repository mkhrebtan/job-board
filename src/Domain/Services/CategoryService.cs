using Domain.Contexts.JobPostingContext.Aggregates;
using Domain.Repos;
using Domain.Shared.ErrorHandling;

namespace Domain.Services;

public class CategoryService
{
    private readonly ICategoryRepository _repository;

    public CategoryService(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<Category>> CreateCategoryAsync(string name, CancellationToken ct)
    {
        var validation = await ValidateNameAsync(name, ct);
        if (validation.IsFailure)
        {
            return Result<Category>.Failure(validation.Error);
        }

        var category = Category.Create(validation.Value.TrimmedName, validation.Value.NormalizedName);
        return Result<Category>.Success(category);
    }

    public async Task<Result> UpdateCategoryNameAsync(Category category, string newName, CancellationToken ct)
    {
        var validation = await ValidateNameAsync(newName, ct);
        if (validation.IsFailure)
        {
            return Result.Failure(validation.Error);
        }

        category.UpdateName(validation.Value.TrimmedName, validation.Value.NormalizedName);
        return Result.Success();
    }

    private async Task<Result<ValidationResult>> ValidateNameAsync(string name, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result<ValidationResult>.Failure(new Error("Category.InvalidName", "Name cannot be empty."));
        }

        var trimmed = name.Trim();
        var normalized = trimmed.ToUpper().Replace(" ", string.Empty);

        if (!await _repository.IsUniqueNameAsync(normalized, ct))
        {
            return Result<ValidationResult>.Failure(new Error("Category.DuplicateName", $"Name '{name}' already exists."));
        }

        return Result<ValidationResult>.Success(new ValidationResult(true, trimmed, normalized));
    }

    private record struct ValidationResult(bool IsValid, string TrimmedName, string NormalizedName);
}