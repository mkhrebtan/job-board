using Application.Abstractions.Messaging;

namespace Application.Commands.Resumes.Languages.Remove;

public record RemoveResumeLanguageCommand(Guid Id, Guid LanguageId) : ICommand;