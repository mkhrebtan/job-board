using Application.Abstractions.Messaging;
using Application.Common.Helpers;
using Domain.Abstraction.Interfaces;
using Domain.Contexts.ResumePostingContext.IDs;
using Domain.Repos.Resumes;
using Domain.Shared.ErrorHandling;
using Domain.Shared.ValueObjects;

namespace Application.Commands.Resumes.Updates.UpdateSkillsDescription;

internal sealed class UpdateResumeSkillsDescriptionHandler : ICommandHandler<UpdateResumeSkillsDescriptionCommand>
{
    private readonly IResumeRepository _resumeRepository;
    private readonly IMarkdownParser _markdownParser;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateResumeSkillsDescriptionHandler(IResumeRepository resumeRepository, IUnitOfWork unitOfWork, IMarkdownParser markdownParser)
    {
        _resumeRepository = resumeRepository;
        _unitOfWork = unitOfWork;
        _markdownParser = markdownParser;
    }

    public async Task<Result> Handle(UpdateResumeSkillsDescriptionCommand command, CancellationToken cancellationToken = default)
    {
        var resume = await _resumeRepository.GetByIdAsync(new ResumeId(command.Id), cancellationToken);
        if (resume is null)
        {
            return Result.Failure(Error.NotFound("Resume.NotFound", "The resume was not found."));
        }

        if (!Helpers.TryCreateVO(() => RichTextContent.Create(command.SkillsDescripitonMarkdown, _markdownParser), out RichTextContent description, out Error error))
        {
            return Result.Failure(error);
        }

        var updateResult = resume.UpdateSkillsDescription(description);
        if (updateResult.IsFailure)
        {
            return Result.Failure(updateResult.Error);
        }

        _resumeRepository.Update(resume);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}