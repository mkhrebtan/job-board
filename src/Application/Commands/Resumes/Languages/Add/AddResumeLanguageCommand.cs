using Application.Abstractions.Messaging;
using Application.Commands.Resumes.Dtos;

namespace Application.Commands.Resumes.Languages.Add;

public record AddResumeLanguageCommand(Guid Id, LanguageSkillDto Language) : ICommand<AddResumeLanguageCommandResponse>;

public record AddResumeLanguageCommandResponse(Guid Id);