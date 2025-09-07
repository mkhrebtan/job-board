using Application.Abstractions.Messaging;
using Domain.Contexts.IdentityContext.Enums;

namespace Application.Commands.Users.Register.JobSeeker;

public record RegisterJobSeekerCommand(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string PhoneNumberRegionCode,
    string Password,
    UserRole UserRole) : ICommand;