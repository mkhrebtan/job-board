using Application.Abstractions.Messaging;

namespace Application.Commands.Resumes.EmploymentTypes.Remove;

public record RemoveResumeEmploymentTypeCommand(Guid Id, ICollection<string> EmploymentTypes) : ICommand;