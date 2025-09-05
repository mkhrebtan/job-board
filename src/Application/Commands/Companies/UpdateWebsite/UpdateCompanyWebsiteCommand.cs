using Application.Abstractions.Messaging;

namespace Application.Commands.Companies.UpdateWebsite;

public record UpdateCompanyWebsiteCommand(Guid Id, string WebsiteUrl) : ICommand;
