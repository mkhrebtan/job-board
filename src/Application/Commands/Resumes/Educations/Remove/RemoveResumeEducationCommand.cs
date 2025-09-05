using Application.Abstractions.Messaging;

namespace Application.Commands.Resumes.Educations.Remove;

public record RemoveResumeEducationCommand(Guid Id, Guid EducationId) : ICommand;