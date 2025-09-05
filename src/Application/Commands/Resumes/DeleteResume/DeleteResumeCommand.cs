using Application.Abstractions.Messaging;

namespace Application.Commands.Resumes.DeleteResume;

public record DeleteResumeCommand(Guid Id) : ICommand;