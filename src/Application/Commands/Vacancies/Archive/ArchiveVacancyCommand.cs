using Application.Abstractions.Messaging;

namespace Application.Commands.Vacancies.Archive;

public record ArchiveVacancyCommand(Guid Id) : ICommand;