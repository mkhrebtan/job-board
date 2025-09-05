using Application.Abstractions.Messaging;

namespace Application.Commands.Companies.UpdateName;

public record UpdateCompanyNameCommand(Guid Id, string Name) : ICommand;