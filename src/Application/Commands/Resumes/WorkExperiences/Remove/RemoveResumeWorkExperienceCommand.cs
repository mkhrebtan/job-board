using Application.Abstractions.Messaging;

namespace Application.Commands.Resumes.WorkExperiences.Remove;

public record RemoveResumeWorkExperienceCommand(Guid Id, Guid WorkExperienceId) : ICommand;