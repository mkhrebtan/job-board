using Domain.Contexts.JobPostingContext.Aggregates;
using FluentValidation;

namespace Application.Commands.Categories.UpdateName;

internal class UpdateCategoryNameCommandValidator : AbstractValidator<UpdateCategoryNameCommand>
{
    public UpdateCategoryNameCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
                .WithMessage("Category name must not be empty.")
            .MaximumLength(Category.MaxNameLength)
                .WithMessage($"Category name must not exceed {Category.MaxNameLength} characters.");
    }
}
