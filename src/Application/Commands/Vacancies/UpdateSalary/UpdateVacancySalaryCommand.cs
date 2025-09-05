using Application.Abstractions.Messaging;
using Application.Commands.Vacancies.Create;

namespace Application.Commands.Vacancies.UpdateSalary;

public record UpdateVacancySalaryCommand(
    Guid Id,
    decimal MinSalary,
    decimal? MaxSalary,
    string SalaryCurrency) : ICommand;