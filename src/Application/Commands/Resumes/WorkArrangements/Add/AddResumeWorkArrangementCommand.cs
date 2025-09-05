using Application.Abstractions.Messaging;

namespace Application.Commands.Resumes.WorkArrangements.Add;

public record AddResumeWorkArrangementCommand(Guid Id, ICollection<string> WorkArrangements) : ICommand;