using Application.Abstractions.Messaging;
using Domain.Contexts.IdentityContext.Enums;

namespace Application.Commands.Users.Register.CompanyEmployee;

public record RegisterCompanyEmployeeCommand(
    Guid CompanyAdminId,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string PhoneNumberRegionCode,
    string Password,
    UserRole UserRole) : ICommand;