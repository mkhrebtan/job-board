using Application.Abstractions.Messaging;

namespace Application.Commands.Companies.UpdateDescription;

public record UpdateCompanyDescriptionCommand(Guid Id, string DescriptionMarkdown) : ICommand;
