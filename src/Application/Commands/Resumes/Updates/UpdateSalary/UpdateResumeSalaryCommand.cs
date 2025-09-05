using Application.Abstractions.Messaging;
using Application.Commands.Resumes.Create;

namespace Application.Commands.Resumes.Updates.UpdateSalary;

public record UpdateResumeSalaryCommand(
    Guid Id,
    decimal SalaryAmount,
    string SalaryCurrency) : ICommand;