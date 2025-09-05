using Application.Abstractions.Messaging;
using Application.Commands.Resumes.Create;

namespace Application.Commands.Resumes.Updates.UpdateSkillsDescription;

public record UpdateResumeSkillsDescription(
    Guid Id,
    string SkillsDescripitonMarkdown) : ICommand;