using Application.Abstractions.Messaging;

namespace Application.Commands.Companies.UpdateLogo;

public record UpdateCompanyLogoCommand(Guid Id, string LogoUrl) : ICommand;
