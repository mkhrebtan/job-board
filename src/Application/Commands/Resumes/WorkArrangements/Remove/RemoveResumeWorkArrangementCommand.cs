using Application.Abstractions.Messaging;

namespace Application.Commands.Resumes.WorkArrangements.Remove;

public record RemoveResumeWorkArrangementCommand(Guid Id, ICollection<string> WorkArrangements) : ICommand;