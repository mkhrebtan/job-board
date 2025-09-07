using Application.Abstractions.Messaging;
using Domain.Contexts.IdentityContext.Enums;

namespace Application.Commands.Users.Register.Employer;

public record RegisterCompanyAdminCommand(
    string FirstName,
    string LastName,
    string CompanyEmail,
    string CompanyName,
    string PhoneNumber,
    string PhoneNumberRegionCode,
    string Password,
    UserRole UserRole) : ICommand;