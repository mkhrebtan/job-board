using Domain.Abstraction;
using Domain.Contexts.JobPostingContext.IDs;
using Domain.Repos;
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

    internal static Category Create(string trimmedName, string normalizedName)
        => new Category(trimmedName, normalizedName);

    internal void UpdateName(string trimmedName, string normalizedName)
    {
        Name = trimmedName;
        NormalizedName = normalizedName;
    }
}
