using Application.Abstractions.Messaging;
using Application.Commands.Resumes.Updates.DraftResume;

namespace Application.Commands.Resumes.Updates.PublishResume;

public record PublishResumeCommand(Guid Id) : ICommand;