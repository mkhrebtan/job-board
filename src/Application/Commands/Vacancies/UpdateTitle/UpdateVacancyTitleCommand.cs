using Application.Abstractions.Messaging;

namespace Application.Commands.Vacancies.UpdateTitle;

public record UpdateVacancyTitleCommand(Guid Id, string Title) : ICommand;