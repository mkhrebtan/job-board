using Application.Abstractions.Messaging;

namespace Application.Commands.Categories.UpdateName;

public record UpdateCategoryNameCommand(Guid Id, string Name) : ICommand;