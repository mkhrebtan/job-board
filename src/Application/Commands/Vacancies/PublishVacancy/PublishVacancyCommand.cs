using Application.Abstractions.Messaging;

namespace Application.Commands.Vacancies.Publish;

public record PublishVacancyCommand(Guid Id) : ICommand;
