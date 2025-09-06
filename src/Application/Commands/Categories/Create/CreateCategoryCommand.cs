using Application.Abstractions.Messaging;

namespace Application.Commands.Categories.Create;

public record CreateCategoryCommand(string Name) : ICommand<CreateCategoryResponse>;

public record CreateCategoryResponse(Guid Id, string Name);
