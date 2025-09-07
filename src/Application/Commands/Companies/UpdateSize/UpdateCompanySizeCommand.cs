using Application.Abstractions.Messaging;

namespace Application.Commands.Companies.UpdateSize;

public record UpdateCompanySizeCommand(Guid Id, int Size) : ICommand;