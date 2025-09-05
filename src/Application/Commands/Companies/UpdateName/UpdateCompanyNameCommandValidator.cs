using Domain.Contexts.RecruitmentContext.Aggregates;
using FluentValidation;

namespace Application.Commands.Companies.UpdateName;

internal class UpdateCompanyNameCommandValidator : AbstractValidator<UpdateCompanyNameCommand>
{
    public UpdateCompanyNameCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(Company.MaxNameLength);
    }
}
