using Application.Abstractions.Messaging;

namespace Application.Commands.Resumes.EmploymentTypes.Add;

public record AddResumeEmploymentTypeCommand(Guid Id, ICollection<string> EmploymentTypes) : ICommand;