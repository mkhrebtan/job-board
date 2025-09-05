using Application.Abstractions.Messaging;

namespace Application.Commands.Vacancies.Delete;

public record DeleteVacancyCommand(Guid Id) : ICommand;