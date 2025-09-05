using Application.Abstractions.Messaging;

namespace Application.Commands.Vacancies.UpdateCategory;

public record UpdateVacancyCategoryCommand(Guid Id, Guid CategoryId) : ICommand;