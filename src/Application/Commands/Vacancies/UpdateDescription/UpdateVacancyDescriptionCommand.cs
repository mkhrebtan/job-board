using Application.Abstractions.Messaging;

namespace Application.Commands.Vacancies.UpdateDescription;

public record UpdateVacancyDescriptionCommand(Guid Id, string descriptionMarkdown) : ICommand;