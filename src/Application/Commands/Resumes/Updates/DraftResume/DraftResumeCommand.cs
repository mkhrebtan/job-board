using Application.Abstractions.Messaging;

namespace Application.Commands.Resumes.Updates.DraftResume;

public record DraftResumeCommand(Guid Id) : ICommand;
