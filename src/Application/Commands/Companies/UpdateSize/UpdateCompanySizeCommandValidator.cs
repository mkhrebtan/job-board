using FluentValidation;

namespace Application.Commands.Companies.UpdateSize;

internal class UpdateCompanySizeCommandValidator : AbstractValidator<UpdateCompanySizeCommand>
{
    public UpdateCompanySizeCommandValidator()
    {
        RuleFor(x => x.Size)
            .GreaterThan(0);
    }
}
