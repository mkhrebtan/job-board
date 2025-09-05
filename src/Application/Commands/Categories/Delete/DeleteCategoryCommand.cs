using Application.Abstractions.Messaging;

namespace Application.Commands.Categories.Delete;

public record DeleteCategoryCommand(Guid Id) : ICommand;
