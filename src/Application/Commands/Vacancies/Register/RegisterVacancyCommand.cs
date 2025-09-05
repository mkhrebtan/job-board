using Application.Abstractions.Messaging;

namespace Application.Commands.Vacancies.Register;

public record RegisterVacancyCommand(Guid Id) : ICommand;
