using Application.Abstractions.Messaging;

namespace Application.Commands.Categories.Create;

public record CreateCategoryCommand(string Name) : ICommand<CreateCategoryRepsonse>;

public record CreateCategoryRepsonse(Guid Id, string Name);
