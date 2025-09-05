using Application.Abstractions.Messaging;

namespace Application.Commands.Resumes.Updates.UpdateDesiredPosition;

public record UpdateResumeDesiredPositionCommand(Guid Id, string DesiredPositionTitle) : ICommand;