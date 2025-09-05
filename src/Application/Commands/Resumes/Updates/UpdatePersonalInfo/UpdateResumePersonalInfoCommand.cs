using Application.Abstractions.Messaging;
using Application.Commands.Resumes.Create;

namespace Application.Commands.Resumes.Updates.UpdatePersonalInfo;

public record UpdateResumePersonalInfoCommand(Guid Id, string FirstName, string LastName) : ICommand;