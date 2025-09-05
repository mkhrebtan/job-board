using Domain.Contexts.JobPostingContext.Aggregates;
using FluentValidation;

namespace Application.Commands.Categories.Create;

internal class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
                .WithMessage("Category name must not be empty.")
            .MaximumLength(Category.MaxNameLength)
                .WithMessage($"Category name must not exceed {Category.MaxNameLength} characters.");
    }
}
