using Application.Abstractions.Messaging;
using Domain.Abstraction.Interfaces;
using Domain.Contexts.JobPostingContext.ValueObjects;
using Domain.Repos.Vacancies;
using Domain.Shared.ErrorHandling;

namespace Application.Commands.Vacancies.UpdateSalary;

internal sealed class UpdateVacancySalaryCommandHandler : ICommandHandler<UpdateVacancySalaryCommand>
{
    private readonly IVacancyRepository _vacancyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateVacancySalaryCommandHandler(IVacancyRepository vacancyRepository, IUnitOfWork unitOfWork)
    {
        _vacancyRepository = vacancyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateVacancySalaryCommand command, CancellationToken cancellationToken = default)
    {
        var vacancy = await _vacancyRepository.GetByIdAsync(command.Id, cancellationToken);
        if (vacancy is null)
        {
            return Result.Failure(Error.NotFound("Vacancy.NotFound", $"Vacancy with ID {command.Id} was not found."));
        }

        Result<Salary> salaryResult = default!;
        if (command.MinSalary == 0 && (command.MaxSalary is null || command.MaxSalary == 0))
        {
            salaryResult = Salary.None();
        }
        else if (command.MinSalary > 0 && (command.MaxSalary is null || command.MaxSalary == command.MinSalary))
        {
            salaryResult = Salary.Fixed(command.MinSalary, command.SalaryCurrency);
        }
        else if (command.MinSalary > 0 && command.MaxSalary is not null && command.MaxSalary == command.MinSalary)
        {
            salaryResult = Salary.Range(command.MinSalary, command.MaxSalary.Value, command.SalaryCurrency);
        }
        else
        {
            return Result.Failure(Error.Problem("CreateVacancyCommandHandler.InvalidSalary", "Invalid salary range provided."));
        }

        if (salaryResult.IsFailure)
        {
            return Result.Failure(salaryResult.Error);
        }

        var result = vacancy.UpdateSalary(salaryResult.Value);
        if (result.IsFailure)
        {
            return Result.Failure(result.Error);
        }

        _vacancyRepository.Update(vacancy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}